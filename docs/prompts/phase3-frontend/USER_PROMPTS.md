# User Prompts - Phase 3: Frontend Implementation

**Date:** 2026-08-08
**Session:** DailyBugle Phase 3 — Frontend Implementation (WPF/MVVM, Admin/User tabs, bug fixes,
test documentation)
**Repository:** chicharito77/InterviewHomework.Sonrisa

> Note: this file lists the verbatim free-text chat messages sent by the user. Intermediate
> confirmations (e.g. approving the `CommunityToolkit.Mvvm` plan, choosing screenshot handling)
> were captured through interactive multiple-choice `ask_user` prompts rather than additional
> free-text messages. Those exchanges are narrated in `SESSION_TRANSCRIPT.md`.

---

## Prompt 1: Phase 3 kickoff

```
Project specific details can be found in "\.github\copilot-instructions.md", please refer to its content.
Original task description can be found in "\docs\task.txt".
Design phase outputs are in ".\docs" folder (ARCHITECTURE.md, PLAN.md, DECISION_LOG.md), with additional files about the backend implementation in the docs folder. 
Phase 2 finished a little while ago. Please go on to phase 3, which is Frontend implementation.
```

---

## Prompt 2: Critical UI bug report

```
There is a mayor problem with the generated UI code. If you start the application, you will see that the different views are collapsed into each other. First, we need to address this huge issue, otherwise the UI is not ready to use
```

---

## Prompt 3: Screenshot deletion complaint + test documentation request

```
oh, why did you delete the screenshots? i wanted to ask you to document the first round of automated tests, inside the testreports folder. Please mark these changes/fixes in a report, similar to phase2 reports. After this, my plan is to manual test it for a short time. And after that, we can save Phase 3, and go onto the Phase 4
```

---

## Prompt 4: Correction — good fix already staged

```
i staged the already good fix, it is there
```

---

## Prompt 5: Further correction — wrong track

```
you are on a wrong track. i staged the already good xaml and cs files before asking you to recreate the testcases.
```

---

## Prompt 6: Manual test results + findings + closure request

```
My manual tests partially passed, please write a documentation about it into the "docs\testreports". Cases were executed sequentially, in one app session, so they are also prerequesities of each other.
TC1: Estebán received a new notification type: Sport. News type added successfully. 
TC2: Trigger Sport news from admin page (Araujo to Liverpool). Both users received the notification. 
TC3: Science news triggered from admin page (mRNA flu vaccine). Both users received the notification. 
TC4: I deleted all news subscription from Estebán except science. I triggered a Sport news from Admin page, only István received it (Rugby news from England)

At the end of the manual test i created 2 screenshots:
".\docs\testreports\images\phase3-round2-gmailInbox.png"
".\docs\testreports\images\phase3-round2-slackChannelState.png"

First finding is that the app starts really slowly. dont leave too much magic waits in the production code. 
Second finding (during TC4): Active checkbox is not editable (temporarily deactivate alarm). This can be logged into the FUTURE_IMPROVEMENTS.
Third finding (during TC1 and TC4): on the Registered users view it is not updated if i add/delete rules from users. Mark it in the FUTURE_IMPROVEMENTS.

Write a report about the manual tests. After that i need to start the closure.
```

---

## Prompt 7: Session export request

```
Export this session to `docs/prompts/phase3-frontend/` with the following three files:

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

**Reference:** See the already existing reports in `docs/prompts/` for example outputs.
```

---

**End of User Prompts**
