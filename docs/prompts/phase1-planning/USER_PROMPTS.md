# User Prompts - Phase 1: Planning & Architecture Design

**Date:** 2026-08-08
**Session:** DailyBugle Phase 1 — Kickoff & Planning / Architecture Design
**Repository:** chicharito77/InterviewHomework.Sonrisa

> Note: this file lists the verbatim free-text chat messages sent by the user. In addition to these,
> several decisions were captured through interactive multiple-choice clarifying questions posed by
> the assistant (e.g., real vs. simulated delivery, user model, simulator triggering mode, rule
> granularity, Teams channel scope, Admin tab scope, history panel). Those questions and the user's
> selected answers are recorded in `SESSION_TRANSCRIPT.md` (Exchange 2 and Exchange 4) and formally
> captured with rationale in `docs/DECISION_LOG.md`.

---

## Prompt 1: Kickoff — planning phase start

```
Lets start to work on my job interview task, with the planning phase. Please find the whole description in "\docs\task.txt".
We already defined important details in "\.github\copilot-instructions.md", please refer to its content.

I am thinking about 5 phases of this development:
- kickoff & planning, architecture design (this is the current phase)
- backend implementation
- frontend implementation
- tests (manual, unittest)
- documentation. 

Additional 
- Architecture decisions should be documented in docs folder (ARCHITECTURE.md).
- Please define PLAN.md into docs folder as well.

Please get to know the project, read the specified files, but don't start planning just yet! After you finished with the first prompt, i want to give you my current understanding of the project. 
After that, we can start the planning phase.

The application's name is DailyBugle.
```

---

## Prompt 2: User's design insights (Phase 1 architecture sketch)

```
Phase 1:Since it would be challenging to import a real world news api, we will implement a separated NewsSimulator in the app.
If i imagine the data structure, I see that the application works with multiple news type (sport; economy; science), and multiple notification channels (email, slack).
Lets prefer in-memory data instead of database setups (short timeframe).
You need to create a DECISIONS_LOG as well to document our process. 

In phase 2, we must use SOLID principles to provide easy-to-test product, even for automated-tests in the future. 
During architecture design we need to ensure that the app will be easily expandable in the future, with new news type or new notification type (teams). 
Solution: DailyBugle.sln. It will be in src folder, with the separate system components separated from each other.  
Core components: 
- domain classes: Event, AlertRule, DateTimeHandler (for being utest friendly)
- channels: INotificationChannel interface, with SlackNotificationChannel and EmailNotificationChannel implementations
- engine: AlertDispatcher

In phase 3, ased on my understanding and the available time for providing a POC for the interviewers, we need to create a simple WPF application, with 2 tabs: admin view (triggering simulation, system monitoring); user view (managing notification settins, handling subscriptions).
- User tab: list of rules, adding new rules (news type, target channel)
- Admin tab: see the list of users; fire event with NewsSimulator

Since I have very limited time, in phase 4 i want to ensure that the available code is testable, easy to maintain, and can be worked further. I would like to define at least 3 test fixtures around the most important core logic.

In the last phase, we will scan through the repository, and adjusting the README documentations, crosslinks between md files. Documentation language will be english. 

Please read my insights above. After that, we can start the discussion.
```

---

## Prompt 3: Approval to draft planning documents

```
Please, proceed
```

---

## Prompt 4: Deep clarification round

```
I reviewed your proposal, and i have some questions for clarification.
DECISION_LOG, D_009: please keep in mind that even if they are not committed to the repo, store these values encrypted!
PLAN:
- user tab will contain a history list too, about the occurred events? 
- can you visualize with a simple ascii art or description how each tabs will look like? 
- i already defined the test users:
user1: Németh István. Needs email notifications (my specified email address, same as the app's registered email address), interested in sport news.
user2: Estebán Alemán. Needs slack notifications (with the already provided slack webhook). Interested in science news.
- since we wont have enough time, right now we wont have user registration form, am i right? I just want to see clearly. 
- i understand execution plan chapter. Right now, our steps will be sequential
- deadline is today, 15:30, so lets update the Time allocation accordingly (i know, i am little bit late).

ARCHITECTURE: 
-  testdata will be stored in DailyBugle.Infrastructure, am i right?
```

---

## Prompt 5: Decision-log ordering fix request

```
Why the ordering seems to be off in the decision_log file? Can it be re-ordered?
```

---

## Prompt 6: "Acting as" identity switcher clarification

```
it is not clear to me what do you mean under "Acting as" user-swither. Now we have 3 users really, right? 2 dummy test users and 1 admin. 

Can you add a visualization of the WPF app mainframe as well? because maybe that way i will understand this concept. 
I expect an "Acting as" dropdown in the top right corner. If the admin is selected, Only the admin tab will be visible. If we switch to the dummy user, admin tab disappears, and only the User tab will be active. 

Please clean this requirement part in the planning documents.
```

---

## Prompt 7: Seed data — shared Science interest

```
one last modification item from my side: i realized that it would be great to set "Németh István" to be interested in Science topics as well. That way we can test the behavior that multiple user is notified about the same event or not. 
And also, add Economy as a unique interest for "Estebán Alemán".
Please update the plan documents accordingly.
```

---

## Prompt 8: Final consistency review request

```
Please go through PLAN, DECISION_LOG and ARCHITECTURE files one more time and check for any ambiguities which may have been made during the plan review process. After that, i will export the session information, and Commit the new files.
```

---

## Prompt 9: Session export request

```
Export this session to `docs/prompts/phase1-planning/` with the following three files:

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
