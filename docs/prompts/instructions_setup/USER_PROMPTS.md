# User Prompts - Instructions Setup Session

**Date:** 2026-08-08  
**Session:** Copilot Instructions File Setup  
**Repository:** chicharito77/InterviewHomework.Sonrisa

---

## Prompt 1: Initial Setup Request

```
I would like to setup the copilot-instructions file properly, to fully align with the purpose of this repository. 
I am working now on a job interview homework. Please process the prompt below, and modify instructions file accordingly.

## 1. Role & Identity
You are acting as a Senior C# / .NET Software Architect and Pair Programmer. Your goal is to write clean, maintainable, production-ready code while explaining your trade-offs clearly.

## 2. Hard Constraints & Workflow Rules (CRITICAL)
- **NO AUTO-COMMITS:** Do NOT execute git commit or push commands under any circumstances without explicit permission.
- **NO UNAPPROVED FILE WRITES:** Before modifying or creating files, briefly explain what you plan to change and ask for my confirmation.
- **HUMAN-IN-THE-LOOP:** Treat every code generation as a draft. Always point out potential risks, assumptions, or edge cases in your proposed solution so I can review them.
- **STEP-BY-STEP EXECUTION:** Do not generate massive multi-file setups at once. Propose changes incrementally.

## 3. Tech Stack & Engineering Standards
- **Platform:** .NET / C# (Modern .NET conventions)
- **Architecture:** Clean Architecture / Domain-Driven concepts
- **Data & Persistence:** In-Memory storage (Repository pattern with thread-safe List/ConcurrentBag, or EF Core In-Memory provider). Keep persistence lightweight—no external database setup required.
- **Design Patterns:** Strategy Pattern for notification channels (`INotificationChannel`), Observer/Pub-Sub for event dispatching, MVVM for WPF.
- **Testing:** NUnit, Moq
- **Code Quality:** SOLID principles, strong typing, explicit error handling, self-documenting naming.

## 4. Response Format
When providing a code solution or plan:
1. State the **Approach & Assumptions** briefly.
2. Present the proposed **Code / Changes**.
3. Highlight **Potential Edge Cases / Risks** for me to validate.

## 5. Communication Style & Brevity (Concise / Direct Mode)
- **NO FLUFF / NO ODYSSEY:** Skip conversational intros, pleasantries, filler text, or lengthy summaries. Get straight to the point.
- **CODE DELTAS ONLY:** When refactoring or updating existing files, show ONLY the changed methods or unified diffs. Do NOT reprint entire unchanged files unless explicitly requested.
- **PUNCHY EXPLANATIONS:** Use short bullet points for reasoning, trade-offs, and edge cases.
- **BE DIRECT:** State what you are doing, why, and what needs my human approval in as few words as possible.
```

---

## Prompt 2: Clarification on Existing File

```
i already have this file in .github folder. Please add WPF application to the Platform details, too, I forgot to list it
```

---

## Prompt 3: Capability Assessment

```
can you recommend other, very important details which can be listed in this file? or it is enough in the current form?
```

---

## Prompt 4: Add Comprehensive Sections

```
Please add the following points to the copilot-instructions.md file (for some chapters i add some more details to include right away).
- Project structure: source code will be stored in src folder. Prompts about each project phase will be exported to prompts folder. 
Decision logs, architecture plans should be stored/recorded in docs folder. Thinking of a structure similar like this:
root/
├── docs/
│   ├── ARCHITECTURE_PLAN.md   
│   ├── DECISION_LOG.md        
│   └── prompts/
│           └── instructions_setup
│           └── planning
│           └── backend_poc
│           └── frontend_poc
├── src/                        
└── README.md      

- Naming Conventions chapter
- Error Handling strategies
- Documentation standards
- Async/await patterns
- C# language feature preferences

Where i did not listed additional info, use your proposals. 
I have a ~6 hour timeframe for this project, today.
```

---

## Prompt 5: Approval to Proceed

```
confirmed
```

---

## Prompt 6: Refinements - Project Name and Framework

```
in chapter 6 dont make a detailed proposal like this at this point. I just wanted to emphasize that all source code must go to there. The project itself will be called "DailyBugle", dont use company name. 

I intend to use .NET core 10. 

please update the copilot-instructions file accordingly.
```

---

## Prompt 7: Export Request

```
Thank you. As a last step, i would like to export this session into the prompts folder, with name 'instructions_setup'.
How can i do this? or can you do this automatically?
```

---

## Prompt 8: Task Requirements Context

```
My task description defines the following: 
Please share your results with us via a GitHub repository that must contain:
•    commits for each major milestone, with meaningful commit messages
•    your prompt history — all prompts used during the process
•    your plans, working artifacts, and evidence of how you worked — design docs, decision logs, prompt drafts, intermediate outputs, notes, scratch files, screenshots, etc.
•    your deliverables — as defined by your own plan

So, based on your answer and the task, i think you should export the session transcript, and a summary document as well, into the prompts folder. 
Collect these files into instructions_setup folder.
```

---

## Prompt 9: Final Confirmation

```
confirm
```

---

## Prompt 10: Full Conversation Export Request

```
These files are solid, but can you also export the current history of this conversation? I need to provide the exact prompt messages i provided to you.
```

---

**End of User Prompts**
