# SQL Seed Scripts

This folder contains SQL scripts for loading curated sample data into Matterway databases.

## Catalog API seed

- SQL loader script: `scripts/sql/catalog/load-data.sql`
- CSV data files:
  - `data/catalog/articles.csv`
  - `data/catalog/details.csv`
  - `data/catalog/article_detail_texts.csv`
  - `data/catalog/article_detail_numerics.csv`
  - `data/catalog/discounts.csv`
  - `data/catalog/article_images.csv` (intentionally empty rows)

### Apply script inside container (if mounted)

```bash
docker exec -i matterway-postgres psql -U postgres -d CatalogDb \
  < scripts/sql/catalog/load-data.sql
```

## Notes

- `load-data.sql` uses PostgreSQL `COPY ... FROM` against files mounted at `/seed-data/catalog/*.csv`.
- It truncates catalog tables and reloads all rows from CSV.
- Article images are intentionally empty in this dataset.
- Data is organized into practical filtering categories:
  - `smart-home-equipment`
  - `pc-components-homelab`
  - `network-equipment`
  - `servers-workstations`
