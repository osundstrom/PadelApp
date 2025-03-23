# Projekt DT191G - PAdelApp

## Info
PadelApp använder sig av ASP.net MVC Core med Entity Framework (EF) för databashantering (SQLite) och Identity för hnatera användare.
På hemsidan kan man skapa konotn, logga in både som admin och vanlig användare. Som användare kna man boka padel tider och radera 
sina egna tider som admin kan man radera och redigera samtliga tider. Som admin går det även att lägga till fler banor. 

I databasen (SQLite) finns det två egna Tabeller som ser ut enligt nedan (samt identity)
 
PadelCourt

| CourtId   | CourtName    | CourtType   | 
| ---- | -------------- | ---------- |
| 1  | Bana 1  | Single   |

PadelBooking

| BookingId   | BookingTime    | CourtId    | UserId  | 
| ---- | -------------- | ---------- | ---------- | 
| 1  | 2025-03-23 | 1   | 1    |


Hemsidan är publicerad via railway. 

