# User Prompts - Export Session Template Setup

**Date:** 2026-08-08  
**Session:** Export Session Template Definition  
**Repository:** chicharito77/InterviewHomework.Sonrisa

---

## Prompt 1: Template Requirement Definition

```
Okay, based on the learned session export rule, define a prompt template, which i can trigger easily at the end of each necessary step. It should export the following files:

Target folder: docs/prompts/[session name]

The prompt command template should be stored in .github/prompts folder? I would not want to place it into doc/prompts.
```

---

## Prompt 2: Template Refinement & Creation

```
Please store the template like .github/EXPORT_SESSION_TEMPLATE.md.
Make the file compact, easy to understand, and always provide the same output. 
You can use now the content of docs\prompts\instructions_setup for a cross-check.
```

---

## Prompt 3: Export This Session (Template Usage)

```
Export this session to `docs/prompts/exportsessiontemplate-setup/` with the following three files:

### 1. SESSION_TRANSCRIPT.md
Full conversation history structured as:
- **Date & Duration** metadata
- **Numbered Exchanges** — each user request + assistant response
- **Summary of Session Outputs** — files created/modified, key decisions

### 2. SUMMARY.md
Outcomes & decisions structured as:
- **Objectives Achieved** — checkmarks for completed goals
- **Key Decisions Made** — decision table with rationale & status
- **Deliverables Ready for Commit** — checkmarks for artifacts
- **Next Steps** — recommended actions for following phases
- **Time Allocation** — if applicable

### 3. USER_PROMPTS.md
All exact user prompts verbatim:
- **Prompt [N]** numbered in order
- Code blocks for structured input (templates, requirements, etc.)
- **End of User Prompts** marker

---

## Context [OPTIONAL]

Provide any additional details about this session:
- Phase focus (planning, backend_poc, frontend_poc, etc.)
- Key deliverables or milestones
- Time spent or constraints

---

**Reference:** See `docs/prompts/instructions_setup/` for example outputs.
```

---

**End of User Prompts**
