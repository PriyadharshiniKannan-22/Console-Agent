# Sample Data & Expected Outputs

## Input Queries and Expected Agent Behaviour

### Query 1: Time

**Input:**
`What time is it in Asia/Kolkata?`

**Expected Tool Call:**

```json
get_time({
  "timezone": "Asia/Kolkata"
})
```

**Expected Tool Result:**

```text
Asia/Kolkata: 2026-06-09 21:04:39
```

**Expected Agent Response:**

> It is 2026-06-09 21:04:39 in Asia/Kolkata.

---

### Query 2: Weather

**Input:**
`What's the weather in Chennai?`

**Expected Tool Call:**

```json
get_weather({
  "city": "Chennai"
})
```

**Expected Tool Result Example:**

```text
Weather in Chennai: 34°C, 72% humidity, scattered clouds
```

**Expected Agent Response:**

> The weather in Chennai is 34°C with 72% humidity and scattered clouds.

---

### Query 3: Employee Database

**Input:**
`Show all employees.`

**Expected Tool Call:**

```sql
SELECT * FROM Employees
```

**Expected Tool Result:**

```text
Id=1 Name=Alice Department=Engineering Salary=85000
Id=2 Name=Bob Department=Marketing Salary=65000
Id=3 Name=Charlie Department=Sales Salary=70000
Id=4 Name=Diana Department=Engineering Salary=95000
```

**Expected Agent Response:**

> Here are all employees in the database:
>
> Alice (Engineering), Bob (Marketing), Charlie (Sales), and Diana (Engineering).

---

### Query 4: Employee Count

**Input:**
`How many employees are in the database?`

**Expected Tool Call:**

```sql
SELECT COUNT(*) FROM Employees
```

**Expected Agent Response:**

> There are 4 employees in the database.

---

### Query 5: Product Database

**Input:**
`List all products.`

**Expected Tool Call:**

```sql
SELECT * FROM Products
```

**Expected Tool Result:**

```text
Id=1 Name=Laptop Price=65000 Stock=10
Id=2 Name=Mouse Price=500 Stock=100
Id=3 Name=Keyboard Price=1500 Stock=50
Id=4 Name=Monitor Price=12000 Stock=20
```

**Expected Agent Response:**

> Here are all products:
>
> Laptop, Mouse, Keyboard, and Monitor.

---

### Query 6: Product Inventory

**Input:**
`Which product has the highest stock?`

**Expected Tool Call:**

```sql
SELECT Name, Stock
FROM Products
ORDER BY Stock DESC
LIMIT 1
```

**Expected Agent Response:**

> Mouse has the highest stock with 100 units available.

---

## Notes

* The agent must call tools instead of answering from memory.
* SQL queries are restricted to SELECT statements only.
* Database query timeout is 3 seconds.
* Database results are capped at 200 rows.
* Tool calling is handled through the Groq Chat Completion API.
