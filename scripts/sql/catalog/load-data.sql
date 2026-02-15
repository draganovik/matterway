-- Catalog API CSV loader
-- Expects CSV files under /seed-data/catalog inside PostgreSQL container.

BEGIN;

TRUNCATE TABLE
    "ArticleDetailNumeric",
    "ArticleDetailText",
    "ArticleImage",
    "Discount",
    "Article",
    "Detail"
CASCADE;

COPY "Detail" ("Slug", "Title", "Unit")
FROM '/seed-data/catalog/details.csv'
WITH (FORMAT csv, HEADER true);

COPY "Article" ("Id", "ArticleCode", "Title", "Description", "BasePrice", "IsAvailable", "CreatedAt", "UpdatedAt")
FROM '/seed-data/catalog/articles.csv'
WITH (FORMAT csv, HEADER true);

COPY "Discount" ("Code", "ArticleId", "Percentage", "ValidFrom", "ValidTo")
FROM '/seed-data/catalog/discounts.csv'
WITH (FORMAT csv, HEADER true);

COPY "ArticleDetailText" ("ArticleId", "DetailSlug", "Value")
FROM '/seed-data/catalog/article_detail_texts.csv'
WITH (FORMAT csv, HEADER true);

COPY "ArticleDetailNumeric" ("ArticleId", "DetailSlug", "Value")
FROM '/seed-data/catalog/article_detail_numerics.csv'
WITH (FORMAT csv, HEADER true);

COPY "ArticleImage" ("Id", "ArticleId", "OrderIndex", "ImageUrl", "ImageAlt")
FROM '/seed-data/catalog/article_images.csv'
WITH (FORMAT csv, HEADER true);

COMMIT;
