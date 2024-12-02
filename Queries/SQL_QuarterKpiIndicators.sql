WITH MAIN_DB AS(
	SELECT 
	b.department,
	c.job,
	a.hiredDatetime,
	CASE
		 WHEN MONTH(a.hiredDatetime) BETWEEN 1 and 3 THEN 'Q1'
		 WHEN MONTH(a.hiredDatetime) BETWEEN 4 and 6 THEN 'Q2'
		 WHEN MONTH(a.hiredDatetime) BETWEEN 7 and 9 THEN 'Q3'
		 WHEN MONTH(a.hiredDatetime) BETWEEN 10 and 12 THEN 'Q4'
	END AS date_quarter

	FROM Employee a
	LEFT JOIN Department b ON (b.departmentId = a.department_id)
	LEFT JOIN Job c ON (c.jobId = a.job_id)

	WHERE YEAR(a.hiredDatetime) = 2021

)

select 
department,
job,
    SUM(CASE WHEN date_quarter = 'Q1' THEN 1 ELSE 0 END) AS Q1,
    SUM(CASE WHEN date_quarter = 'Q2' THEN 1 ELSE 0 END) AS Q2, 
    SUM(CASE WHEN date_quarter = 'Q3' THEN 1 ELSE 0 END) AS Q3,
    SUM(CASE WHEN date_quarter = 'Q4' THEN 1 ELSE 0 END) AS Q4
FROM  MAIN_DB
GROUP BY department,job
ORDER BY department,job