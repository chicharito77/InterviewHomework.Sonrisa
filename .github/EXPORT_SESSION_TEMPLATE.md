# Export Session: [SESSION_NAME]

**Instructions:** Fill in [SESSION_NAME] below, provide any additional context, then submit.

---

## Export Request

Export this session to `docs/prompts/[SESSION_NAME]/` with the following three files:

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
