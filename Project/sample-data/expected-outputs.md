# Sample Data & Expected Outputs

## Input Queries and Expected Agent Behaviour

### Query 1: Weather
**Input:** `What is the weather in Chennai right now?`

**Expected Tool Call:** `get_weather({ "city": "Chennai" })`

**Expected Output:**
```
City: Chennai | Temp: 34.2°C | Humidity: 72% | Condition: partly cloudy
```
**Agent Response (example):**
> The current weather in Chennai is 34.2°C with 72% humidity. Conditions are partly cloudy.

---

### Query 2: Time
**Input:** `What time is it in Tokyo?`

**Expected Tool Call:** `get_time({ "timezone": "Asia/Tokyo" })`

**Expected Output:**
```
2026-06-08 17:45:03 JST
```
**Agent Response:**
> It is currently 5:45 PM in Tokyo (Japan Standard Time).

---

### Query 3: Database — Top Employees
**Input:** `Who are the top 3 employees by sales this quarter?`

**Expected Tool Call:**
```sql
SELECT name, sales FROM employees ORDER BY sales DESC LIMIT 3
```

**Expected Output:**
```json
[
  {"name": "Priya Sharma", "sales": 142000},
  {"name": "Arjun Nair",   "sales": 138500},
  {"name": "Meera Iyer",   "sales": 125000}
]
```

---

### Query 4: Database — Products
**Input:** `List all electronics products under ₹5000`

**Expected Tool Call:**
```sql
SELECT name, price, stock FROM products WHERE category = 'Electronics' AND price < 5000
```

**Expected Output:**
```json
[
  {"name": "Wireless Mouse", "price": 1200, "stock": 150},
  {"name": "USB-C Hub",      "price": 3500, "stock": 87}
]
```

---

### Query 5: Multi-tool (bonus)
**Input:** `What's the weather in London and what time is it there?`

**Expected Tool Calls (sequential):**
1. `get_weather({ "city": "London" })`
2. `get_time({ "timezone": "Europe/London" })`
