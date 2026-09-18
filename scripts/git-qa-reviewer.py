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


def get_available_ollama_model() -> tuple[bool, str, list[str]]:
    """
    Checks if Ollama is running and returns (is_running, resolved_model_name, available_models).
    """
    try:
        req = urllib.request.Request(f"{OLLAMA_ENDPOINT}/api/tags", method="GET")
        with urllib.request.urlopen(req, timeout=3) as response:
            if response.status != 200:
                return False, "", []
            data = json.loads(response.read().decode("utf-8"))
            models = [m.get("name", "") for m in data.get("models", [])]
            
            # Check configured target model
            target_model = OLLAMA_MODEL
            if target_model in models:
                return True, target_model, models
            
            # Match base name (e.g. qwen2.5-coder without tag or with :latest)
            target_base = target_model.split(":")[0]
            for m in models:
                if m == target_base or m.startswith(f"{target_base}:"):
                    return True, m, models
            
            # Match any coder or qwen model
            for m in models:
                if "qwen" in m.lower() or "coder" in m.lower():
                    return True, m, models
            
            if models:
                return True, models[0], models
            
            return True, "", []
    except Exception:
        return False, "", []


def get_git_info() -> tuple[str, str, str]:
    """Returns (repo_root, current_branch, commit_author)"""
    try:
        root = subprocess.run(["git", "rev-parse", "--show-toplevel"], capture_output=True, text=True, check=True).stdout.strip()
        branch = subprocess.run(["git", "rev-parse", "--abbrev-ref", "HEAD"], capture_output=True, text=True, check=True).stdout.strip()
        author = subprocess.run(["git", "config", "user.name"], capture_output=True, text=True, check=True).stdout.strip() or "Developer"
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
        "You are an expert .NET/C# QA Engineer and Senior Code Reviewer acting as a strict Git pre-commit gatekeeper.\n" +
        "Your task is to analyze staged Git diffs and explicitly APPROVE or REJECT the commit based on quality and performance.\n\n" +
        
        "### CONTEXT\n" +
        "Stack: C#, ASP.NET Core, EF Core, MediatR, CQRS, Clean Architecture.\n\n" +
        
        "### REVIEW CRITERIA (Focus exclusively on these):\n" +
        "1. Performance & Efficiency: Unnecessary allocations, N+1 queries, missing AsNoTracking, blocking async (.Result/.Wait()), or sync-over-async.\n" +
        "2. Code Quality & Logic: Null reference risks, unhandled exceptions, swallowing exceptions, mutating state in MediatR queries, or Clean Architecture boundary violations.\n" +
        "3. Maintainability: Highly complex methods, massive code duplication, or improper dependency injection.\n" +
        "IGNORE cosmetic spacing, styling, variable naming, and security/auth checks.\n\n" +
        
        "### INSTRUCTIONS\n" +
        "1. Read the provided git diff carefully.\n" +
        "2. Analyze the code based purely on the Review Criteria.\n" +
        "3. If critical logic flaws, memory leaks, performance bottlenecks, or boundary violations exist, you MUST REJECT.\n" +
        "4. If the code is efficient, logically sound, and contains zero blocking issues, you MUST APPROVE.\n\n" +
        
        "### OUTPUT FORMAT\n" +
        "You must respond EXACTLY in the following Markdown structure. Do not add introductory conversational text.\n\n" +
        
        "### 🔍 Summary\n" +
        "[1-2 sentences summarizing the architectural or logic changes]\n\n" +
        
        "### ⚙️ Analysis\n" +
        "[Briefly think through the code against the criteria. Note how the changes impact performance or maintainability.]\n\n" +
        
        "### 🚀 QA & Performance Issues\n" +
        "- [Bullet points of actionable bugs or bottlenecks found, or '- None identified']\n\n" +
        
        "### 💡 Maintainability Suggestions\n" +
        "- [1-2 structural or efficiency improvements, or '- None']\n\n" +
        
        "### 🎯 Verdict\n" +
        "[VERDICT: APPROVE] or [VERDICT: REJECT]"
    )


def stream_review_from_ollama(model_name: str, diff_text: str, files: list[str]) -> tuple[str, str]:
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
        "model": model_name,
        "messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        "stream": True,
        "options": {
            "temperature": 0.1,  # Low temperature for deterministic, high-accuracy analysis
            "top_p": 0.85,
        }
    }

    req = urllib.request.Request(
        f"{OLLAMA_ENDPOINT}/api/chat",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )

    full_response = ""
    print(f"\n{BOLD}{CYAN}🤖 QA Agent ({model_name}) reviewing {len(files)} staged file(s)...{RESET}\n")
    if truncated:
        print(f"{DIM}(Note: Large diff truncated to first {MAX_DIFF_CHARS} chars){RESET}\n")

    try:
        with urllib.request.urlopen(req, timeout=120) as response:
            for line in response:
                if not line:
                    continue
                chunk = json.loads(line.decode("utf-8"))
                token = chunk.get("message", {}).get("content", "") or chunk.get("response", "")
                sys.stdout.write(token)
                sys.stdout.flush()
                full_response += token
        print("\n")
    except Exception as e:
        print(f"\n{YELLOW}⚠️ Error during Ollama inference: {e}{RESET}\n")
        return "", "ERROR"

    # Extract Verdict (Fix 1)
    verdict = "APPROVE"
    if "[VERDICT: REJECT]" in full_response or "VERDICT: REJECT" in full_response:
        verdict = "REJECT"
    elif "[VERDICT: APPROVE]" in full_response or "VERDICT: APPROVE" in full_response:
        verdict = "APPROVE"
    elif any(k in full_response.lower() for k in ["potential bug", "security risk", "memory leak", "critical issue", "boundary violation"]):
        verdict = "REJECT"

    return full_response, verdict


def save_qa_report(repo_root: str, branch: str, model_name: str, files: list[str], review_text: str, verdict: str):
    """Saves the latest review report to .git/LAST_QA_REPORT.md"""
    git_dir = os.path.join(repo_root, ".git")
    if not os.path.exists(git_dir):
        return

    report_path = os.path.join(git_dir, "LAST_QA_REPORT.md")
    now_str = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")

    # Fix 2
    badge = "🟢 **PASSED (APPROVE)**" if verdict == "APPROVE" else "🔴 **REJECTED (ISSUES REPORTED)**"

    report_content = f"""# 🤖 Local QA Pre-Commit Report

- **Date:** `{now_str}`
- **Branch:** `{branch}`
- **Model:** `{model_name}`
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
            # Fix 3
            if verdict == "APPROVE":
                tty_out.write(f"{GREEN}{BOLD}✅ QA Verdict: APPROVED! Proceeding with commit...{RESET}\n\n")
                return True
            else:
                tty_out.write(f"{YELLOW}{BOLD}⚠️  QA Review REJECTED this commit based on issues above.{RESET}\n")
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

    is_running, resolved_model, available_models = get_available_ollama_model()

    if not is_running:
        print(f"{DIM}[QA Hook] Ollama is not reachable at {OLLAMA_ENDPOINT}. Skipping AI check.{RESET}")
        return 0

    if not resolved_model:
        print(f"{YELLOW}[QA Hook] Ollama is running, but no suitable Qwen/Coder model was found.{RESET}")
        print(f"{DIM}[QA Hook] To enable AI pre-commit reviews, run: `ollama pull qwen2.5-coder:7b`{RESET}")
        return 0

    repo_root, branch, _ = get_git_info()

    try:
        diff_text, files = get_staged_diff()
    except subprocess.CalledProcessError as e:
        print(f"{YELLOW}[QA Hook] Could not read git diff: {e}{RESET}")
        return 0

    if not diff_text.strip() or not files:
        return 0

    response, verdict = stream_review_from_ollama(resolved_model, diff_text, files)
    if verdict == "ERROR":
        return 0

    # Save to .git/LAST_QA_REPORT.md
    save_qa_report(repo_root, branch, resolved_model, files, response, verdict)

    proceed = prompt_user_confirmation(verdict)
    if not proceed:
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())