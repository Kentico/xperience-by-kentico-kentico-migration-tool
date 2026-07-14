-- =============================================================================
-- PopulateEffectiveMetadata.sql
--
-- Run this script against the SOURCE (KX13) database BEFORE starting the
-- migration tool.  It creates (or re-creates) the auxiliary table
-- [dbo].[Migration_EffectiveMetadata] and fills it with the EFFECTIVE values
-- of the three SEO metadata fields for every document whose page type has
-- ClassHasMetadata = 1.
-- =============================================================================

-- Drop and re-create so the script is safe to re-run.
IF OBJECT_ID('dbo.Migration_EffectiveMetadata', 'U') IS NOT NULL
    DROP TABLE dbo.Migration_EffectiveMetadata;

-- Recursive CTE walks the page tree top-down, propagating each effective value
-- to children that have NULL in their own column.
WITH Hierarchy AS (

    -- Anchor: root nodes (NodeLevel = 0) seed the chain with their own values.
    SELECT
        v.DocumentID,
        v.NodeID,
        v.NodeParentID,
        v.NodeLevel,
        v.NodeSiteID,
        v.DocumentGUID,
        v.DocumentCulture,
        v.ClassName,
        v.NodeClassID,
        v.DocumentPageTitle       AS EffectiveTitle,
        v.DocumentPageKeyWords    AS EffectiveKeywords,
        v.DocumentPageDescription AS EffectiveDescription,
        CAST(0 AS BIT)            AS TitleInherited,
        CAST(0 AS BIT)            AS KeywordsInherited,
        CAST(0 AS BIT)            AS DescriptionInherited
    FROM dbo.View_CMS_Tree_Joined v
    WHERE v.NodeLevel = 0

    UNION ALL

    -- Recursive member: each child uses its parent's resolved effective value
    -- whenever its own column is NULL.
    SELECT
        child.DocumentID,
        child.NodeID,
        child.NodeParentID,
        child.NodeLevel,
        child.NodeSiteID,
        child.DocumentGUID,
        child.DocumentCulture,
        child.ClassName,
        child.NodeClassID,
        ISNULL(child.DocumentPageTitle,       h.EffectiveTitle),
        ISNULL(child.DocumentPageKeyWords,    h.EffectiveKeywords),
        ISNULL(child.DocumentPageDescription, h.EffectiveDescription),
        CAST(CASE WHEN child.DocumentPageTitle       IS NULL THEN 1 ELSE 0 END AS BIT),
        CAST(CASE WHEN child.DocumentPageKeyWords    IS NULL THEN 1 ELSE 0 END AS BIT),
        CAST(CASE WHEN child.DocumentPageDescription IS NULL THEN 1 ELSE 0 END AS BIT)
    FROM Hierarchy h
    INNER JOIN dbo.View_CMS_Tree_Joined child
        ON  child.NodeParentID    = h.NodeID
        AND child.DocumentCulture = h.DocumentCulture   -- stay within the same culture branch
)

-- Materialise only the pages whose class has ClassHasMetadata = 1.
SELECT
    h.DocumentID,
    h.NodeID,
    h.NodeSiteID,
    h.DocumentGUID,
    h.ClassName,
    h.EffectiveTitle,
    h.TitleInherited,
    h.EffectiveKeywords,
    h.KeywordsInherited,
    h.EffectiveDescription,
    h.DescriptionInherited
INTO dbo.Migration_EffectiveMetadata
FROM Hierarchy h
INNER JOIN dbo.CMS_Class c ON h.NodeClassID = c.ClassID
WHERE c.ClassHasMetadata = 1
