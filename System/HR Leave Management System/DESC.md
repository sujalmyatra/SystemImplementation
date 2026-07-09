## HR Leave Management System

The HR Leave Management System is designed to manage employees, leave types, leave applications, approvals, leave balance, and leave history. The system allows employees to apply for leave, managers or HR to approve/reject leave requests, and the system maintains leave balance automatically.

### Main Entities

The system will have the following main entities:

1. **Employee**
   An employee represents a person working in the company.

2. **Leave Type**
   A leave type represents different categories of leaves.

   Examples:

   * Casual Leave
   * Sick Leave
   * Paid Leave
   * Unpaid Leave
   * Emergency Leave

3. **Leave Request**
   A leave request represents an application submitted by an employee for leave.

4. **Leave Approval**
   A leave approval represents the approval or rejection decision made by manager or HR.

5. **Leave Balance**
   Leave balance represents how many leaves are available for an employee for each leave type.

---

### Relationships

1. **Employee to Leave Request**
   One employee can apply for multiple leave requests.

2. **Leave Type to Leave Request**
   One leave type can be used in multiple leave requests.

3. **Employee to Leave Balance**
   One employee can have multiple leave balances based on leave types.

4. **Leave Request to Leave Approval**
   One leave request can have one approval decision.

5. **Manager to Leave Approval**
   One manager can approve or reject multiple leave requests.

---

### Leave Request Status

The system should support the following leave request statuses:

1. **Pending**
   Leave request is submitted but not approved/rejected yet.

2. **Approved**
   Leave request is approved by manager or HR.

3. **Rejected**
   Leave request is rejected by manager or HR.

4. **Cancelled**
   Leave request is cancelled by the employee.

---

### Leave Rules

The system should allow employees to apply for leave.

1. Employee can apply for leave by selecting leave type, start date, end date, and reason.

2. Leave start date should not be greater than leave end date.

3. Leave days should be calculated automatically.

Example:

Start Date = 10 July 2026
End Date = 12 July 2026

Total Leave Days = 3

4. Employee should not be able to apply for leave if leave balance is insufficient.

5. Employee should not have overlapping leave requests for the same dates.

Example:

If Employee A already applied leave from 10 July to 12 July, then another leave from 11 July to 13 July should not be allowed.

6. Rejected leave should not reduce leave balance.

7. Cancelled leave should not reduce leave balance.

---

### Leave Approval Rules

The system should allow manager or HR to approve/reject leave requests.

1. Only pending leave requests can be approved or rejected.

2. Approved leave should reduce the employee’s leave balance.

3. Rejected leave should keep the leave balance unchanged.

4. Leave approval should store approver name, approval date, status, and remarks.

5. Already approved leave should not be approved again.

6. Already rejected leave should not be approved again.

Example:

Employee A has 10 Casual Leaves.
Employee A applies for 2 Casual Leaves.
If approved, remaining Casual Leave balance becomes 8.

---

### Leave Balance Rules

The system should maintain leave balance for each employee and leave type.

1. Each employee can have separate balance for each leave type.

Example:

Employee A:

* Casual Leave: 10
* Sick Leave: 8
* Paid Leave: 15

2. Leave balance should decrease only after leave approval.

3. Leave balance should not become negative.

4. Unpaid leave can be allowed even if paid leave balance is not available.

5. Leave balance can be reset yearly.

Example:

At the start of every year, Casual Leave can reset to 10 and Sick Leave can reset to 8.

---

### Business Rules

1. Employee cannot apply for leave with invalid date range.

2. Employee cannot apply for leave if leave balance is insufficient.

3. Employee cannot apply for overlapping leave dates.

4. Only pending leave requests can be approved or rejected.

5. Approved leave should reduce leave balance.

6. Rejected leave should not reduce leave balance.

7. Cancelled leave should not reduce leave balance.

8. Leave history should not be permanently deleted.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Employee
* Leave Type
* Leave Request
* Leave Approval
* Leave Balance

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve employee leave history and approval records.

---

### Reports

The system should provide the following reports:

#### 1. Employee-wise Leave Report

Show how many leaves each employee has taken in a selected date range.

Example:

* Employee A: 5 leaves
* Employee B: 3 leaves

---

#### 2. Leave Type-wise Report

Show leave count grouped by leave type.

Example:

* Casual Leave: 50
* Sick Leave: 30
* Paid Leave: 20

---

#### 3. Pending Leave Request Report

Show leave requests that are still pending for approval.

Example:

* Employee A: Casual Leave from 10 July to 12 July
* Employee B: Sick Leave from 15 July to 16 July

---

#### 4. Leave Balance Report

Show available leave balance for each employee.

Example:

Employee A:

* Casual Leave: 8
* Sick Leave: 5
* Paid Leave: 12

---

#### 5. Approved Leave Report

Show approved leaves in a selected month.

Example:

July 2026:

* Employee A: 2 approved leaves
* Employee B: 4 approved leaves

---

#### 6. Rejected Leave Report

Show rejected leave requests with reason or remarks.

Example:

* Employee A: Rejected due to project deadline
* Employee B: Rejected due to insufficient balance

---

#### 7. Employees With Low Leave Balance Report

Show employees whose leave balance is less than a fixed limit.

Example:

If Casual Leave balance is less than 2, show employee in low balance report.

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show employee-wise approved leave count for last month.

2. Show leave-type-wise leave count.

3. Show pending leave requests with employee name and leave type.

4. Show employee-wise leave balance.

5. Show employees whose leave balance is less than 2.

6. Show rejected leave requests in current month.

7. Show employees who took more than 5 leaves in last month.

8. Show employees who have no leave requests in current month.

9. Show approval count manager-wise.

10. Show overlapping leave requests for an employee.

11. Show approved leaves ordered by latest approval date.

12. Show leave requests status-wise.

---

### Overall System Goal

The main goal of this system is to manage employee leave requests, approvals, leave balance, and leave reports. The system should prevent invalid leave applications, avoid overlapping leave requests, maintain leave balance correctly, and preserve leave history for HR and management reporting.
