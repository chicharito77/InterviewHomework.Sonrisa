# Session Transcript: Export Session Template Setup

**Date:** 2026-08-08  
**Duration:** ~20 minutes  
**Objective:** Define reusable prompt template for standardized session exports across project phases

---

## Exchange 1: Template Requirement

**USER:**
Okay, based on the learned session export rule, define a prompt template, which i can trigger easily at the end of each necessary step. It should export the following files:

Target folder: docs/prompts/[session name]

The prompt command template should be stored in .github/prompts folder? I would not want to place it into doc/prompts.

**ASSISTANT:**
Proposed `.github/prompts/EXPORT_SESSION_TEMPLATE.md` with:
- `[SESSION_NAME]` placeholder for phases
- Three standard exports: SESSION_TRANSCRIPT, SUMMARY, USER_PROMPTS
- Guidance on what to capture in each file

Asked for confirmation on template location and detail level.

---

## Exchange 2: Template Refinement & Creation

**USER:**
Please store the template like .github/EXPORT_SESSION_TEMPLATE.md.
Make the file compact, easy to understand, and always provide the same output. 
You can use now the content of docs\prompts\instructions_setup for a cross-check.

**ASSISTANT:**
Confirmed simplified location and approach.

**ACTION:** Created `.github/EXPORT_SESSION_TEMPLATE.md` with:
- `[SESSION_NAME]` placeholder
- Three file definitions (SESSION_TRANSCRIPT, SUMMARY, USER_PROMPTS)
- Format specifications matching instructions_setup example
- Compact structure, easy to copy+paste
- Reference to existing example outputs

---

## Exchange 3: Template Usage

**USER:**
[Submitted export template with `exportsessiontemplate-setup` as session name]

**ASSISTANT:**
Processing export using template rules.

**ACTIONS:**
- Created `docs/prompts/exportsessiontemplate-setup/` folder
- Generated SESSION_TRANSCRIPT.md (this file)
- Generated SUMMARY.md (outcomes & decisions)
- Generated USER_PROMPTS.md (exact user prompts)

---

## Summary of Session Outputs

### Files Created:
1. `.github/EXPORT_SESSION_TEMPLATE.md` — Reusable export prompt template
2. `docs/prompts/exportsessiontemplate-setup/SESSION_TRANSCRIPT.md` — This file
3. `docs/prompts/exportsessiontemplate-setup/SUMMARY.md` — Outcomes & decisions
4. `docs/prompts/exportsessiontemplate-setup/USER_PROMPTS.md` — Exact user prompts

### Key Achievements:
- ✅ Template created with consistent output format
- ✅ Easy copy+paste mechanism for phase-end exports
- ✅ Standardized structure across all sessions
- ✅ Example outputs documented for reference
