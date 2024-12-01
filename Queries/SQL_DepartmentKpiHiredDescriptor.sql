	WITH MEAN_TABLE AS (
	SELECT
	department,
	COUNT(0) SIZE
	FROM Employee a
	LEFT JOIN Department b ON (b.id = a.department_id)
	WHERE YEAR(a.hiredDatetime) = 2021
	GROUP BY department
	)
	
	
	SELECT 
	b.id,
	b.department,
	COUNT(0) hired
	FROM Employee a 
	LEFT JOIN Department b ON (b.departmentId = a.department_id)
	GROUP BY department, b.id
	HAVING COUNT(0) > (SELECT AVG(SIZE) FROM MEAN_TABLE)
	ORDER BY hired desc

	