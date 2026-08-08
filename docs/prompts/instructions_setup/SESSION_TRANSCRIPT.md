# Session Transcript: Instructions Setup

**Date:** 2026-08-08  
**Duration:** ~28 minutes  
**Objective:** Establish Copilot instructions file aligned with interview homework requirements

---

## Exchange 1: Initial Setup Request

**USER:**
I would like to setup the copilot-instructions file properly, to fully align with the purpose of this repository. I am working now on a job interview homework. Please process the prompt below, and modify instructions file accordingly.

[User provided comprehensive requirements covering:]
- Role & Identity: Senior C# / .NET Software Architect and Pair Programmer
- Hard Constraints: NO AUTO-COMMITS, NO UNAPPROVED FILE WRITES, HUMAN-IN-THE-LOOP, STEP-BY-STEP EXECUTION
- Tech Stack: .NET / C# (Modern), Clean Architecture, In-Memory storage, Strategy/Observer/MVVM patterns, NUnit/Moq, SOLID principles
- Response Format: Approach → Code → Risks
- Communication Style: Concise, direct, no fluff, code deltas only

**ASSISTANT:**
Proposed to create `.copilot-instructions` file at repository root with all requirements encoded. Suggested risks: filename correctness, global application scope. Asked for confirmation to proceed.

---

## Exchange 2: Existing File Clarification

**USER:**
I already have this file in .github folder. Please add WPF application to the Platform details, too, I forgot to list it.

**ASSISTANT:**
Located existing `.github/copilot-instructions.md` (generic repository state tracking). Proposed creating new file with full requirements + WPF. User clarified to merge into existing file instead.

**ACTION:** Merged interview homework workflow into `.github/copilot-instructions.md`, removed duplicate.

---

## Exchange 3: Enhancement Recommendations

**USER:**
Can you recommend other, very important details which can be listed in this file? Or is it enough in the current form?

**ASSISTANT:**
Proposed 8 additional categories:
1. Project Structure / Folder Conventions
2. Naming Conventions
3. Error Handling Strategy
4. Dependency Injection Setup
5. Git Workflow
6. Documentation Standards
7. Async/Await Patterns
8. C# Language Feature Preferences

Recommended highest ROI items (1–4), offered to draft minimal or comprehensive set.

---

## Exchange 4: Comprehensive Section Additions

**USER:**
Please add the following points to copilot-instructions.md file [with specific details]:
- Project structure: source code in `src/`, prompts in `prompts/`, decision logs in `docs/`
- Specific folder structure provided for root layout
- Naming Conventions chapter
- Error Handling strategies
- Documentation standards
- Async/await patterns
- C# language feature preferences
- Note: ~6 hour timeframe for this project

**ASSISTANT:**
Confirmed additions and requested approval before editing.

**ACTION:** Added 6 new sections (6–11) to copilot-instructions.md with detailed proposals for each category + time constraint note.

---

## Exchange 5: Refinement - Project Name & Framework

**USER:**
In chapter 6 don't make detailed proposal at this point. Just emphasize all source code must go to `src/`. Project is called "DailyBugle", not company name. Using .NET Core 10. Update accordingly.

**ASSISTANT:**
Simplified Section 6 to minimal guidance (src/ emphasis only). Updated namespace examples from `Sonrisa` → `DailyBugle`. Updated target framework from .NET 6 → .NET Core 10.

**ACTIONS:**
- Simplified Section 6 structure guidance
- Changed all `Sonrisa` namespace examples to `DailyBugle`
- Updated target framework to .NET Core 10

---

## Exchange 6: Session Export

**USER:**
As a last step, export this session into prompts folder with name 'instructions_setup'. How can I do this or can you do it automatically?

**ASSISTANT:**
Asked for clarification on export format (transcript, summary, or both).

**USER:** 
Shared task requirements showing need for prompt history, plans, working artifacts. Requested both session transcript AND summary document in `docs/prompts/instructions_setup/` folder.

**ASSISTANT:**
Confirmed approach and proceeded with creation.

---

## Summary of Session Outputs

### Files Created/Modified:
1. `.github/copilot-instructions.md` — Full workflow instructions with 11 sections
2. `docs/prompts/instructions_setup/SESSION_TRANSCRIPT.md` — This file
3. `docs/prompts/instructions_setup/SUMMARY.md` — Outcomes & decisions

### Key Decisions:
- Merged interview homework workflow into existing repo instructions file
- Emphasized scope discipline over detailed architectural examples
- Project name: "DailyBugle" (no company branding)
- Target: .NET Core 10 (latest)
- 6-hour time constraint noted for prioritization

### Files Ready for Commitment:
- `.github/copilot-instructions.md` (complete workflow guidelines)
- `docs/` folder structure (ready for architecture docs & decision logs)
- `src/` folder (ready for implementation)
