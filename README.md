
# Matterway - Smart IoT Store
[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)

Projekat Matterway predstavlja inovativno rešenje za prevazilaženje prethodno navedenih problema. Fokusiran je na prodaju i proizvodnju lako podesivih smart home/office/hotel uređaja koji se jednostavno instaliraju i odlično komuniciraju međusobno, budući da se koriste istim standardom. Ovo omogućava korisnicima da sami biraju koje uređaje žele i u kojoj količini žele da opreme svoj prostor. 

![Screenshot 2023-06-05 102306](https://github.com/draganovik/Matterway/assets/15861333/2f585995-ec38-4406-8d18-ac24fc4f795d)

## Database and Migration setup

```bash
  cd Matterway\src\Services
```

than run the cmd executables in next order:

1. databases_drop.cmd
2. migrations_remove.cmd
3. migrations_add.cmd
4. databases_update.cmd

## Tech Stack

**Orchestration:** Docker, Docker Compose

**Database:** Microsoft SQL Server 2019

**Server:** ASP.NET 7 API, Entity Framework Core (EFCore 7), Swagger

**Client:** Nuxt.js, Stripe, Tailwind CSS, Flowbite, Pinia, Prettier

## Authors

- Mladen Draganović [@draganovik](https://www.github.com/draganovik)
