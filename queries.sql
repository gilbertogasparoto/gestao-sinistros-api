-- 1. Ranking de ramos de seguro com maior percentual de sinistros negados nos últimos 6 meses

SELECT  
	CASE a.ramo_seguro
        WHEN 1 THEN 'Automóvel'
        WHEN 2 THEN 'Residencial'
        WHEN 3 THEN 'Vida'
        WHEN 4 THEN 'Empresarial'
    END AS ramo,
	COUNT(*) AS total_sinistros, 
	SUM(CASE WHEN s.status = 5 THEN 1 ELSE 0 END) AS negados,
	ROUND(SUM(CASE WHEN s.status = 5 THEN 1 ELSE 0 END)::numeric / COUNT(*) * 100, 2) AS percentual_negados
FROM sinistros s
JOIN apolices a ON a.id = s.apolice_id
WHERE s.created_at >= CURRENT_DATE - INTERVAL '6 months'
GROUP BY ramo
ORDER BY percentual_negados DESC;

-- 2. Top 10 clientes com maior valor estimado de sinistros em análise ou aprovados

SELECT
    c.nome,
    SUM(s.valor_estimado) AS total_estimado
FROM clientes c
JOIN apolices a
    ON a.cliente_id = c.id
JOIN sinistros s
    ON s.apolice_id = a.id
WHERE s.status IN (2,3)
GROUP BY c.id, c.nome
ORDER BY total_estimado DESC
LIMIT 10;

