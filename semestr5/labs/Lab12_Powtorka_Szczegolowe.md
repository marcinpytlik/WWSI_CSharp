# Lab12 Powtórka i hardening API

**Czas:** 90 min

## Checklista

- [ ] DTO na wejściu i wyjściu, nie gołe encje z nawigacją
- [ ] kody HTTP świadome (nie wszystko 200)
- [ ] walidacja → 400, brak → 404, brak autoryzacji → 401, brak uprawnień → 403
- [ ] JWT sekret poza git
- [ ] logi bez haseł i tokenów
- [ ] testy integracyjne na CI (`dotnet test`)
- [ ] `global.json` / TFM `net10.0`

## Mini-audyt (praca na własnym projekcie)

Znajdź i napraw **dwie** rzeczy z listy. Wpisz je w `AUDIT.md` w projekcie (nie w tym repo, chyba że to PR).
