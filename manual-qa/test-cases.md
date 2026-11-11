# Test cases: public booking form

**Project:** Booking Website (pet project), page `/appointments/book`
**Environment:** local Docker build, Chrome, http://localhost:8000
**Common preconditions:** one service "Document consultation" exists; free slots exist on 16.11.2026 at 10:00 and 11:00;
the test date is at least 12 hours in the future (the site does not allow booking closer than 12 hours ahead).

| ID | Title | Priority | Type | Last result |
|---|---|---|---|---|
| TC-01 | Successful booking with valid data | High | Positive | Passed |
| TC-02 | Booked time disappears from the time list | High | Positive | Passed |
| TC-03 | Phone number with letters is rejected | Medium | Negative | Not run |
| TC-04 | Booking without privacy consent is rejected | High | Negative | Not run |
| TC-05 | Second booking with the same email keeps the first appointment's details | High | Negative | **Failed → [BUG-001](bug-report.md)** |

---

## TC-01: Successful booking with valid data

**Steps**
1. Open `/appointments/book`.
2. Enter name `Anna Koval`, email `family.qa@example.com`, phone `+380501112233`.
3. Choose service `Document consultation`, date `16.11.2026`, time `10:00`.
4. Tick "I agree to the privacy policy" and click **Book**.

**Expected result**
- The "Request sent!" page opens (`/appointments/success`).
- A "Booking request received" email addressed to `family.qa@example.com` appears in Mailpit (http://localhost:8025).
- The dashboard shows the appointment with status **pending** and the entered name, phone, service, date and time.

## TC-02: Booked time disappears from the time list

**Precondition:** TC-01 is done (10:00 on 16.11.2026 is booked).

**Steps**
1. Open `/appointments/book`.
2. Choose date `16.11.2026` and open the **Time** list.

**Expected result**
- Only `11:00` is offered; `10:00` is not in the list.

## TC-03: Phone number with letters is rejected

**Steps**
1. Fill in all fields with valid data, but enter phone `+38050abc1122`.
2. Click **Book**.

**Expected result**
- The form is not sent; the error "Enter a phone number with 7-15 digits, e.g. +15550100123." is shown under the phone field.
- No appointment is created; the chosen time is still free.

## TC-04: Booking without privacy consent is rejected

**Steps**
1. Fill in all fields with valid data.
2. Leave "I agree to the privacy policy" unticked and click **Book**.

**Expected result**
- The form is not sent; the message "You need to accept the privacy policy." is shown.
- No appointment is created.

## TC-05: Second booking with the same email keeps the first appointment's details

**Precondition:** TC-01 is done (Anna Koval, `+380501112233`, 10:00).

**Steps**
1. Open `/appointments/book`.
2. Enter name `Oleh Koval`, the **same** email `family.qa@example.com`, phone `+380679998877`.
3. Choose date `16.11.2026`, time `11:00`, tick the consent box and click **Book**.
4. Open **Dashboard → Appointments**.

**Expected result**
- 10:00: **Anna Koval, +380501112233**.
- 11:00: **Oleh Koval, +380679998877**.

**Actual result (2026-11-11):** both appointments show Oleh Koval, +380679998877. See [BUG-001](bug-report.md).
