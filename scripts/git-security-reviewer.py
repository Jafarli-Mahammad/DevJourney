#!/usr/bin/env python3
"""
AI Pre-Commit Security Reviewer powered by local Qwen2.5-Coder (7B) via Ollama.
(DeepSeek-R1 14B configuration preserved as commented-out option)
Dedicated to detecting security vulnerabilities, authorization issues (BOLA/BFLA),
hardcoded secrets, injection vectors, and untrusted data handling in .NET/C#.
"""

import os
import re
import sys
import json
import fnmatch
import datetime
import subprocess
import urllib.request
import urllib.error

# Configuration
OLLAMA_ENDPOINT = os.environ.get("OLLAMA_ENDPOINT", "http://localhost:11434")

# DeepSeek-R1 (14B) - commented out:
# OLLAMA_MODEL = os.environ.get("OLLAMA_SECURITY_MODEL", "deepseek-r1:14b")

# Active Model: Qwen2.5-Coder
OLLAMA_MODEL = os.environ.get("OLLAMA_SECURITY_MODEL", "qwen2.5-coder:7b")

MAX_DIFF_CHARS = int(os.environ.get("MAX_DIFF_CHARS", "18000"))
REQUEST_TIMEOUT = int(os.environ.get("SECURITY_REVIEW_TIMEOUT", "240"))

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
MAGENTA = "\033[95m"
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
                print(f"{DIM}[Security Hook] Ollama returned HTTP status {response.status}. Skipping AI check.{RESET}")
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

            # DeepSeek fallback (commented out):
            # for m in models:
            #     if "deepseek" in m.lower():
            #         return True, m, models
            
            return True, "", models
    except urllib.error.URLError as e:
        print(f"{DIM}[Security Hook] Ollama connection error ({e.reason}) at {OLLAMA_ENDPOINT}. Skipping AI check.{RESET}")
        return False, "", []
    except json.JSONDecodeError as e:
        print(f"{DIM}[Security Hook] Failed to parse Ollama tags response as JSON: {e}. Skipping AI check.{RESET}")
        return False, "", []
    except Exception as e:
        print(f"{DIM}[Security Hook] Unexpected error discovering Ollama models: {e}. Skipping AI check.{RESET}")
        return False, "", []


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


def strip_thinking(text: str) -> str:
    """Remove <think>...</think> reasoning blocks (if present) before verdict parsing."""
    return re.sub(r"<think>.*?</think>", "", text, flags=re.DOTALL).strip()


def extract_thinking(text: str) -> str:
    """Pull out the <think>...</think> reasoning block, if present, for reporting purposes."""
    match = re.search(r"<think>(.*?)</think>", text, flags=re.DOTALL)
    return match.group(1).strip() if match else ""


def build_system_prompt() -> str:
    return (
        "You are an elite DevSecOps Gatekeeper and Application Security Auditor.\n"
        "Your task is to analyze staged Git diffs strictly for genuine, exploitable security vulnerabilities and explicit secret leaks.\n\n"

        "### CRITICAL GROUNDING RULE\n"
        "You must base your entire analysis STRICTLY on the literal code shown in the diff below. "
        "Do NOT infer, assume, or reference any library, API, or provider not explicitly present in the diff text. "
        "If the diff is too small or ambiguous to assess meaningfully, state that explicitly and default to APPROVE "
        "rather than inventing context.\n\n"

        "### CONTEXT\n"
        "Stack: C#, ASP.NET Core (Identity), EF Core, MediatR, CQRS, Clean Architecture.\n\n"

        "### SECURITY REVIEW CRITERIA:\n"
        "1. Auth & BOLA (IDOR): Missing [Authorize] attributes, bypassing MediatR pipeline behaviors for auth, or missing tenant/user ownership validation before data access.\n"
        "2. Injection: Using EF Core 'FromSqlRaw' with string interpolation (SQLi), unvalidated file paths (Directory Traversal), or unsafe deserialization.\n"
        "3. Secrets: ACTUAL hardcoded production API keys, JWT symmetric keys, or DB passwords.\n"
        "4. Cryptography & Privacy: Custom crypto implementations, or logging plaintext PII/passwords.\n\n"

        "### FALSE POSITIVE SUPPRESSION (CRITICAL):\n"
        "- IGNORE dependency injection boilerplate, localhost URLs, and standard routing.\n"
        "- IGNORE references to secrets via IConfiguration or Environment variables (these are safe).\n"
        "- IGNORE test data, mock variables, and appsettings.Development.json configurations.\n"
        "- DO NOT hallucinate vulnerabilities. Assume standard ASP.NET Core secure defaults are active unless the diff explicitly overrides them.\n\n"

        "### INSTRUCTIONS\n"
        "1. Read the diff and identify the attack surface.\n"
        "2. Think step-by-step in the Threat Analysis section before concluding.\n"
        "3. If genuine, exploitable vulnerabilities or exposed production secrets exist, you MUST REJECT.\n"
        "4. If the code introduces no real attack surface, you MUST APPROVE.\n\n"

        "### OUTPUT FORMAT\n"
        "You must respond EXACTLY in the following Markdown structure:\n\n"

        "### 🛡️ Security Assessment Summary\n"
        "[1-2 sentences summarizing the attack surface changes]\n\n"

        "### ⚙️ Threat Analysis\n"
        "[Briefly model the threat. Think step-by-step: Is authorization enforced? Is input sanitized? Are secrets actually exposed?]\n\n"

        "### 🚨 Vulnerabilities\n"
        "- [Actionable bullet points of genuine exploits/leaks, or '- None identified']\n\n"

        "### 🔒 Hardening Suggestions\n"
        "- [Defense-in-depth suggestions, or '- None']\n\n"

        "### 🎯 Verdict\n"
        "[VERDICT: APPROVE] or [VERDICT: REJECT]"
    )


def stream_review_from_ollama(model_name: str, diff_text: str, files: list[str]) -> tuple[str, str]:
    truncated = False
    if len(diff_text) > MAX_DIFF_CHARS:
        diff_text = diff_text[:MAX_DIFF_CHARS] + "\n\n... [Diff truncated: exceeding max token limit for security review] ..."
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
            "temperature": 0.1,
            "top_p": 0.85,
            # "num_ctx": 16384,  # DeepSeek extended context window (commented out)
        },
        "keep_alive": 0  # Unload immediately after review so GPU memory is freed
    }

    req = urllib.request.Request(
        f"{OLLAMA_ENDPOINT}/api/chat",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )

    full_response = ""
    in_think_block = False
    print(f"\n{BOLD}{MAGENTA}🛡️  Security Agent ({model_name}) reviewing {len(files)} staged file(s)...{RESET}\n")
    if truncated:
        print(f"{DIM}(Note: Large diff truncated to first {MAX_DIFF_CHARS} chars){RESET}\n")

    try:
        with urllib.request.urlopen(req, timeout=REQUEST_TIMEOUT) as response:
            for line in response:
                if not line:
                    continue
                chunk = json.loads(line.decode("utf-8"))
                token = chunk.get("message", {}).get("content", "") or chunk.get("response", "")
                full_response += token

                # Dim the reasoning trace if <think> blocks are present
                if "<think>" in token:
                    in_think_block = True
                if "</think>" in token:
                    in_think_block = False
                    sys.stdout.write(RESET)
                    sys.stdout.flush()
                    continue

                sys.stdout.write((DIM if in_think_block else RESET) + token)
                sys.stdout.flush()
        print(f"{RESET}\n")
    except Exception as e:
        print(f"\n{YELLOW}⚠️ Error during Ollama security review: {e}{RESET}\n")
        return "", "ERROR"

    # Verdict parsing ignores any <think> reasoning block if present
    answer_only = strip_thinking(full_response)

    verdict = "APPROVE"
    if not answer_only.strip():
        verdict = "REJECT"
    elif "[VERDICT: REJECT]" in answer_only or "VERDICT: REJECT" in answer_only:
        verdict = "REJECT"
    elif "[VERDICT: APPROVE]" in answer_only or "VERDICT: APPROVE" in answer_only:
        verdict = "APPROVE"
    elif any(k in answer_only.lower() for k in ["recommend rejecting", "reject this commit", "critical vulnerability", "sql injection", "idor vulnerability", "hardcoded credentials"]):
        verdict = "REJECT"

    return full_response, verdict


def save_security_report(repo_root: str, branch: str, model_name: str, files: list[str], review_text: str, verdict: str):
    git_dir = os.path.join(repo_root, ".git")
    if not os.path.exists(git_dir):
        return

    report_path = os.path.join(git_dir, "LAST_SECURITY_REPORT.md")
    now_str = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    badge = "🟢 **PASSED (APPROVE)**" if verdict == "APPROVE" else "🔴 **REJECTED (SECURITY RISKS FOUND)**"

    answer = strip_thinking(review_text)
    reasoning = extract_thinking(review_text)
    reasoning_block = (
        f"\n<details>\n<summary>🧠 Model Reasoning (click to expand)</summary>\n\n{reasoning}\n\n</details>\n"
        if reasoning else ""
    )

    report_content = f"""# 🛡️ Local Security Pre-Commit Report

- **Date:** `{now_str}`
- **Branch:** `{branch}`
- **Model:** `{model_name}`
- **Status:** {badge}

---

### 📂 Staged Files ({len(files)})
""" + "\n".join(f"- `{f}`" for f in files) + f"""

---

{answer.strip()}
{reasoning_block}
---
*Generated automatically by `.git/hooks/pre-commit` via Ollama.*
"""

    try:
        with open(report_path, "w", encoding="utf-8") as f:
            f.write(report_content)
        print(f"{DIM}📄 Full Security report saved to: .git/LAST_SECURITY_REPORT.md{RESET}")
    except Exception as e:
        print(f"{DIM}[Security Hook] Could not save report file: {e}{RESET}")


def prompt_user_confirmation(verdict: str) -> bool:
    if verdict == "APPROVE":
        print(f"{GREEN}{BOLD}✅ Security Verdict: APPROVED! No blocking security risks detected.{RESET}\n")
        return True

    print(f"{RED}{BOLD}🛑 Security Review REJECTED this commit based on risks above.{RESET}\n")
    tty_path = "/dev/tty"
    try:
        if os.path.exists(tty_path):
            with open(tty_path, "r") as tty_in, open(tty_path, "w") as tty_out:
                tty_out.write(f"{BOLD}Proceed with commit anyway? [y/N]: {RESET}")
                tty_out.flush()
                answer = tty_in.readline().strip().lower()
                if answer in ("y", "yes"):
                    tty_out.write(f"{YELLOW}Bypassing security rejection upon user request.{RESET}\n\n")
                    return True
                else:
                    tty_out.write(f"{RED}Commit aborted due to security rejection.{RESET}\n\n")
                    return False
    except Exception:
        pass

    # If non-interactive (no TTY available to prompt)
    print(f"{RED}Non-interactive session: Aborting commit due to security rejection.{RESET}\n")
    return False


def main():
    if os.environ.get("SKIP_SECURITY_REVIEW") == "1" or os.environ.get("SKIP_QA") == "1":
        return 0

    is_running, resolved_model, available_models = get_available_ollama_model()

    if not is_running:
        return 0

    if not resolved_model:
        available_str = f" (installed: {', '.join(available_models)})" if available_models else ""
        print(f"{YELLOW}[Security Hook] Ollama is running, but no suitable Qwen/Coder model was found{available_str}.{RESET}")
        print(f"{DIM}[Security Hook] To enable AI pre-commit security reviews, run: `ollama pull qwen2.5-coder:7b`{RESET}")
        # print(f"{DIM}[Security Hook] For DeepSeek-R1: `ollama pull deepseek-r1:14b`{RESET}")
        return 0

    repo_root, branch, _ = get_git_info()

    try:
        diff_text, files = get_staged_diff()
    except subprocess.CalledProcessError as e:
        print(f"{YELLOW}[Security Hook] Could not read git diff: {e}{RESET}")
        return 0

    if not diff_text.strip() or not files:
        return 0

    response, verdict = stream_review_from_ollama(resolved_model, diff_text, files)
    if verdict == "ERROR":
        return 0

    save_security_report(repo_root, branch, resolved_model, files, response, verdict)

    proceed = prompt_user_confirmation(verdict)
    if not proceed:
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())