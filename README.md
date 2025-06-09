# KursSistemDiplomskiRad

## Funkcionalnosti

### Za korisnike
- Pregled dostupnih kurseva
- Prijava na kurs
- Pregled vlastitih kurseva
- Prijava i registracija korisnika
- Praćenje napretka kroz kurseve

### Za administratore
- Dodavanje, uređivanje i brisanje kurseva
- Upravljanje korisnicima i njihovim registracijama
- Pregled statistike i izvještaja

## Administratorske privilegije

Prijava za administratore se vrši korištenjem sljedećih podataka:
- **Email:** admin@gmail.com
- **Password:** admin123

## Korištene tehnologije

- **Entity Framework Core**: Za rad s bazom podataka i implementaciju ORM-a.
- **JWT (JSON Web Tokens)**: Za autentifikaciju i autorizaciju korisnika.
- **Dependency Injection**: Implementirano kroz registraciju servisa u Program.cs, omogućava modularnost i lakšu zamenu implementacija.
- **AutoMapper**: Koristi se za mapiranje objekata između slojeva aplikacije.
- **EF Core LINQ Queries**: Za rad sa podacima u bazama koristeći upite zasnovane na LINQ.
- **Rad sa datotekama**: Koristi se kod lekcija za upload medijskog sadržaja i upravljanje datotekama.
- **Microsoft SQL Server**: Baza podataka za pohranu podataka.
