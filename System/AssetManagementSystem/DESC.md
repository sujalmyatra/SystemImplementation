## Asset Management System

The Asset Management System is designed to manage company assets, employees, asset assignments, assignment history, and maintenance records. The system keeps track of which asset is assigned to which employee, asset condition, maintenance status, and asset availability.

### Main Entities

The system will have the following main entities:

1. **Asset**
   An asset represents a company-owned item such as laptop, mobile, monitor, keyboard, chair, vehicle, or printer.

2. **Employee**
   An employee represents a person working in the company who can receive company assets.

3. **Asset Assignment**
   An asset assignment represents the current allocation of an asset to an employee.

4. **Assignment History**
   Assignment history stores past assignment records of assets.

5. **Maintenance Record**
   Maintenance record represents repair, service, or maintenance details of an asset.

---

### Relationships

1. **Employee to Asset Assignment**
   One employee can have multiple assigned assets.

2. **Asset to Asset Assignment**
   One asset can be assigned to one employee at a time.

3. **Asset to Assignment History**
   One asset can have multiple assignment history records over time.

4. **Employee to Assignment History**
   One employee can have multiple asset assignment history records.

5. **Asset to Maintenance Record**
   One asset can have multiple maintenance records.

---

### Asset Status

The system should support the following asset statuses:

1. **Available**
   Asset is not assigned to anyone and can be assigned.

2. **Assigned**
   Asset is currently assigned to an employee.

3. **UnderMaintenance**
   Asset is under repair or service.

4. **Damaged**
   Asset is damaged and cannot be assigned.

5. **Retired**
   Asset is no longer used by the company.

---

### Asset Assignment Rules

The system should allow assets to be assigned to employees.

1. Only available assets can be assigned to employees.

2. One asset cannot be assigned to two employees at the same time.

3. One employee can have multiple assets.

4. When an asset is assigned, its status should become Assigned.

5. When an asset is returned, its status should become Available or UnderMaintenance based on condition.

Example:

If Laptop A is already assigned to Employee X, it cannot be assigned to Employee Y until it is returned.

---

### Asset Return Rules

The system should allow employees to return assigned assets.

1. Only assigned assets can be returned.

2. Return date should be stored.

3. Asset condition should be checked during return.

4. If asset is in good condition, status should become Available.

5. If asset is damaged, status should become Damaged or UnderMaintenance.

6. Returned asset should be added to assignment history.

Example:

Employee A returns Laptop X.
If Laptop X is working properly, status becomes Available.
If Laptop X has damage, status becomes UnderMaintenance.

---

### Maintenance Rules

The system should maintain repair and service records.

1. Maintenance can be created only for existing assets.

2. Asset under maintenance should not be assigned to employees.

3. Maintenance record should contain issue description, maintenance date, cost, and status.

4. After maintenance is completed, asset status should become Available or Damaged based on result.

5. Maintenance cost should be greater than or equal to 0.

Example:

Laptop A has a screen issue.
Maintenance record is created with issue description and cost.
After repair, asset becomes Available.

---

### Business Rules

1. Asset code should be unique.

2. Damaged or retired assets should not be assigned.

3. Under-maintenance assets should not be assigned.

4. One asset should have only one active assignment.

5. Assignment history should be preserved.

6. Maintenance history should be preserved.

7. Returned assets should update asset status properly.

8. Retired assets should not be assigned or sent for regular assignment.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Asset
* Employee
* Asset Assignment
* Assignment History
* Maintenance Record

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve asset history, employee assignment records, and maintenance records.

---

### Reports

The system should provide the following reports:

#### 1. Employee-wise Asset Report

Show assets assigned to each employee.

Example:

Employee A:

* Laptop
* Mouse
* Keyboard

Employee B:

* Mobile
* Monitor

---

#### 2. Asset Status Report

Show asset count status-wise.

Example:

* Available: 20
* Assigned: 50
* UnderMaintenance: 5
* Damaged: 3
* Retired: 2

---

#### 3. Asset Assignment History Report

Show past assignment history of a selected asset.

Example:

Laptop A:

* Assigned to Employee X from 1 Jan to 10 Mar
* Assigned to Employee Y from 15 Mar to 20 June

---

#### 4. Maintenance Cost Report

Show total maintenance cost in a selected month.

Example:

July 2026 maintenance cost = ₹25,000

---

#### 5. Assets Under Maintenance Report

Show assets currently under maintenance.

Example:

* Laptop A: Screen issue
* Printer B: Paper jam issue

---

#### 6. Damaged Asset Report

Show assets marked as damaged.

Example:

* Laptop X
* Mobile Y

---

#### 7. Unassigned Asset Report

Show assets that are available and not assigned to any employee.

Example:

* Laptop A
* Monitor B
* Keyboard C

---

#### 8. Employee Asset Return Report

Show assets returned by employees in a selected date range.

Example:

* Employee A returned Laptop X on 5 July 2026
* Employee B returned Mobile Y on 8 July 2026

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show employee-wise assigned asset count.

2. Show asset-status-wise count.

3. Show assets currently assigned to a selected employee.

4. Show available assets that are not assigned to anyone.

5. Show assets under maintenance.

6. Show total maintenance cost for current month.

7. Show asset assignment history for a selected asset.

8. Show employees who have more than 3 active assets.

9. Show damaged assets.

10. Show assets not assigned in the last 6 months.

11. Show most frequently maintained assets.

12. Show returned assets in last month.

---

### Overall System Goal

The main goal of this system is to manage company assets, employee asset assignments, return history, and maintenance tracking. The system should prevent assigning unavailable assets, maintain assignment history, track asset condition, preserve maintenance records, and generate useful asset reports for company management.
