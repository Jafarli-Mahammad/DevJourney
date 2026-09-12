#!/usr/bin/env python3
"""
AI Pre-Commit QA Reviewer powered by local Ollama (Qwen2.5-Coder 7B).
Optimized specifically for 7B parameter reasoning, C#/.NET Clean Architecture,
and high-signal code quality checks.
"""

import os
import sys
import json
import fnmatch
import datetime
import subprocess
import urllib.request
import urllib.error

# Configuration
OLLAMA_ENDPOINT = os.environ.get("OLLAMA_ENDPOINT", "http://localhost:11434")
OLLAMA_MODEL = os.environ.get("OLLAMA_MODEL", "qwen2.5-coder:7b")
MAX_DIFF_CHARS = int(os.environ.get("MAX_DIFF_CHARS", "18000"))

# File exclusion patterns
IGNORE_PATTERNS = [
    "*.lock", "*-lock.json", "*-lock.yaml", "package-lock.json", "pnpm-lock.yaml", "yarn.lock", "Cargo.lock",
    "packages.lock.json", "*.min.js", "*.min.css", "*.map", "*.svg", "*.png", "*.jpg", "*.jpeg", "*.gif",
    "*.ico", "*.wasm", "*.dll", "*.exe", "*.so", "*.dylib", "*.db", "*.sqlite", "*.log", "*.suo",
    "*.DotSettings.user", "*.DotSettings", ".idea/*", ".vscode/*", "zap-report.html", "codebase-memory.db",
    "*.nupkg", "obj/*", "bin/*"
]

# ANSI Colors
CYAN = "\033[96m"
GREEN = "\033[92m"
YELLOW = "\033[93m"
RED = "\033[91m"
BOLD = "\033[1m"
DIM = "\033[2m"
RESET = "\033[0m"


def is_ignored(filename: str) -> bool:
    basename = os.path.basename(filename)
    for pattern in IGNORE_PATTERNS:
        if fnmatch.fnmatch(filename, pattern) or fnmatch.fnmatch(basename, pattern):
            return True
    return False


def check_ollama_status() -> bool:
    try:
        req = urllib.request.Request(f"{OLLAMA_ENDPOINT}/api/tags", method="GET")
        with urllib.request.urlopen(req, timeout=2) as response:
            return response.status == 200
    except Exception:
        return False


def get_git_info() -> tuple[str, str, str]:
    """Returns (repo_root, current_branch, commit_author)"""
    try:
        root = subprocess.run(["git", "rev-parse", "--show-toplevel"], capture_output=True, text=True, check=True).stdout.strip()
        branch = subprocess.run(["git", "rev-parse", "--abbrev-ref", "HEAD"], capture_output=True, text=True, check=True).stdout.strip()
        author = subprocess.run(["git", "config", "user.name"], capture_output=True, text=True).stdout.strip() or "Developer"
        return root, branch, author
    except Exception:
        return ".", "unknown", "Developer"


def get_staged_diff() -> tuple[str, list[str]]:
    cmd_files = ["git", "diff", "--cached", "--name-only", "--diff-filter=ACMR"]
    res_files = subprocess.run(cmd_files, capture_output=True, text=True, check=True)
    staged_files = [f.strip() for f in res_files.stdout.splitlines() if f.strip()]

    candidate_files = [f for f in staged_files if not is_ignored(f)]

    if not candidate_files:
        return "", []

    cmd_diff = ["git", "diff", "--cached", "--unified=3", "--"] + candidate_files
    res_diff = subprocess.run(cmd_diff, capture_output=True, text=True, check=True)
    return res_diff.stdout, candidate_files


def build_system_prompt() -> str:
    """
    Highly structured prompt optimized for Qwen2.5-Coder 7B.
    Guides the model with strict domain rules, anti-hallucination guardrails, and .NET awareness.
    """
    return (
        "You are an expert QA Engineer and Senior Code Reviewer performing a Git Pre-Commit review.\n"
        "The project is a C# / .NET backend (Clean Architecture, ASP.NET Core, EF Core, MediatR, CQRS).\n\n"
        "### YOUR TASK:\n"
        "Analyze the staged git diff for critical logic bugs, security risks, and performance issues.\n\n"
        "### GUIDELINES FOR ACCURACY:\n"
        "1. DO NOT comment on styling, indentation, cosmetic spacing, or obvious framework boilerplate.\n"
        "2. ONLY report high-signal, actionable issues with clear technical explanations.\n"
        "3. Pay special attention to:\n"
        "   - C# async anti-patterns (e.g., .Result, .Wait(), sync-over-async blocking, missing cancellation tokens).\n"
        "   - Nullability issues (dereferencing nullable types without checks, missing null guards).\n"
        "   - EF Core pitfalls (missing AsNoTracking on read queries, unintended client-side evaluation, unindexed filters).\n"
        "   - Security (hardcoded secrets, tokens, connection strings, unvalidated user input).\n"
        "   - Exception handling (swallowing exceptions, empty catch blocks, throwing generic System.Exception).\n\n"
        "### RESPONSE FORMAT:\n"
        "Use exactly this Markdown structure:\n"
        "### 🔍 Summary\n"
        "Brief 1-2 sentence overview of what is changing.\n\n"
        "### 🐛 Bugs & Edge Cases\n"
        "- Bullet points of potential runtime bugs or logic flaws (or '- None identified').\n\n"
        "### ⚡ Performance & Resource Leaks\n"
        "- Bullet points of query inefficiency, allocation/memory issues, or leaks (or '- None identified').\n\n"
        "### 🔒 Security & Secrets\n"
        "- Bullet points of exposed keys, injection risks, or auth flaws (or '- None identified').\n\n"
        "### 💡 Recommendations\n"
        "- 1-2 key suggestions if any, otherwise '- None'.\n\n"
        "### 🎯 Verdict\n"
        "End with exactly one of:\n"
        "[VERDICT: LGTM] (if no bugs or only minor non-blocking suggestions)\n"
        "[VERDICT: WARNINGS] (if potential logic bugs, security risks, or notable performance issues exist)\n"
    )


def stream_review_from_ollama(diff_text: str, files: list[str]) -> tuple[str, str]:
    truncated = False
    if len(diff_text) > MAX_DIFF_CHARS:
        diff_text = diff_text[:MAX_DIFF_CHARS] + "\n\n... [Diff truncated: exceeding max token limit for pre-commit review] ..."
        truncated = True

    system_prompt = build_system_prompt()
    user_prompt = (
        f"Staged Files ({len(files)}):\n" + "\n".join(f"- `{f}`" for f in files) +
        f"\n\n```diff\n{diff_text}\n```"
    )

    payload = {
        "model": OLLAMA_MODEL,
        "prompt": f"<|im_start|>system\n{system_prompt}<|im_end|>\n<|im_start|>user\n{user_prompt}<|im_end|>\n<|im_start|>assistant\n",
        "stream": True,
        "options": {
            "temperature": 0.1,  # Low temperature for deterministic, high-accuracy analysis
            "top_p": 0.85,
        }
    }

    req = urllib.request.Request(
        f"{OLLAMA_ENDPOINT}/api/generate",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )

    full_response = ""
    print(f"\n{BOLD}{CYAN}🤖 QA Agent ({OLLAMA_MODEL}) reviewing {len(files)} staged file(s)...{RESET}\n")
    if truncated:
        print(f"{DIM}(Note: Large diff truncated to first {MAX_DIFF_CHARS} chars){RESET}\n")

    try:
        with urllib.request.urlopen(req, timeout=90) as response:
            for line in response:
                if not line:
                    continue
                chunk = json.loads(line.decode("utf-8"))
                token = chunk.get("response", "")
                sys.stdout.write(token)
                sys.stdout.flush()
                full_response += token
        print("\n")
    except Exception as e:
        print(f"\n{YELLOW}⚠️ Error during Ollama inference: {e}{RESET}\n")
        return "", "ERROR"

    # Extract Verdict
    verdict = "LGTM"
    if "[VERDICT: WARNINGS]" in full_response or "VERDICT: WARNINGS" in full_response:
        verdict = "WARNINGS"
    elif "[VERDICT: LGTM]" in full_response or "VERDICT: LGTM" in full_response:
        verdict = "LGTM"
    elif any(k in full_response.lower() for k in ["potential bug", "security risk", "memory leak", "critical issue"]):
        verdict = "WARNINGS"

    return full_response, verdict


def save_qa_report(repo_root: str, branch: str, files: list[str], review_text: str, verdict: str):
    """Saves the latest review report to .git/LAST_QA_REPORT.md"""
    git_dir = os.path.join(repo_root, ".git")
    if not os.path.exists(git_dir):
        return

    report_path = os.path.join(git_dir, "LAST_QA_REPORT.md")
    now_str = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    badge = "🟢 **PASSED (LGTM)**" if verdict == "LGTM" else "🟡 **WARNINGS REPORTED**"

    report_content = f"""# 🤖 Local QA Pre-Commit Report

- **Date:** `{now_str}`
- **Branch:** `{branch}`
- **Model:** `{OLLAMA_MODEL}`
- **Status:** {badge}

---

### 📂 Staged Files ({len(files)})
""" + "\n".join(f"- `{f}`" for f in files) + f"""

---

{review_text.strip()}

---
*Generated automatically by `.git/hooks/pre-commit` via Ollama.*
"""

    try:
        with open(report_path, "w", encoding="utf-8") as f:
            f.write(report_content)
        print(f"{DIM}📄 Full QA report saved to: .git/LAST_QA_REPORT.md{RESET}")
    except Exception as e:
        print(f"{DIM}[QA Hook] Could not save report file: {e}{RESET}")


def prompt_user_confirmation(verdict: str) -> bool:
    tty_path = "/dev/tty"
    if not os.path.exists(tty_path):
        return True

    try:
        with open(tty_path, "r") as tty_in, open(tty_path, "w") as tty_out:
            if verdict == "LGTM":
                tty_out.write(f"{GREEN}{BOLD}✅ QA Verdict: LGTM! Proceeding with commit...{RESET}\n\n")
                return True
            else:
                tty_out.write(f"{YELLOW}{BOLD}⚠️  QA Review highlighted potential warnings above.{RESET}\n")
                tty_out.write(f"{BOLD}Proceed with commit anyway? [Y/n]: {RESET}")
                tty_out.flush()
                answer = tty_in.readline().strip().lower()
                if answer in ("", "y", "yes"):
                    tty_out.write(f"{GREEN}Proceeding with commit.{RESET}\n\n")
                    return True
                else:
                    tty_out.write(f"{RED}Commit aborted by user.{RESET}\n\n")
                    return False
    except Exception:
        return True


def main():
    if os.environ.get("SKIP_QA") == "1":
        return 0

    if not check_ollama_status():
        print(f"{DIM}[QA Hook] Ollama not reachable at {OLLAMA_ENDPOINT}. Skipping AI check.{RESET}")
        return 0

    repo_root, branch, _ = get_git_info()

    try:
        diff_text, files = get_staged_diff()
    except subprocess.CalledProcessError as e:
        print(f"{YELLOW}[QA Hook] Could not read git diff: {e}{RESET}")
        return 0

    if not diff_text.strip() or not files:
        return 0

    response, verdict = stream_review_from_ollama(diff_text, files)
    if verdict == "ERROR":
        return 0

    # Save to .git/LAST_QA_REPORT.md
    save_qa_report(repo_root, branch, files, response, verdict)

    proceed = prompt_user_confirmation(verdict)
    if not proceed:
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
