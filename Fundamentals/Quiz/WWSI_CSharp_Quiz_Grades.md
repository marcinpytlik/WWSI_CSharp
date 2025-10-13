# 🧮 Arkusz oceny – C# Quiz (zdane/niezdane)

---

## Wariant domyślny: wagi **L=1**, **M=2**, **H=3**
- Liczba pytań: **L=5**, **M=18**, **H=7** (razem 30)
- Maksimum punktów: `5*1 + 18*2 + 7*3 = 62`
- **Próg zaliczenia:** `≥ 37 pkt` (tj. 60% z 62)

### Pola do wypełnienia
| Grupa | Poprawnych | Waga | Punkty = Poprawnych × Waga |
|------:|-----------:|-----:|---------------------------:|
| L     |            | 1    |                            |
| M     |            | 2    |                            |
| H     |            | 3    |                            |
| **Suma** |        |      |                            |

**Wzór:**  
`Wynik = L*1 + M*2 + H*3`  
**Werdykt:**  
`zdane` jeśli `Wynik ≥ 37` • w przeciwnym razie `niezdane`

---

## Wariant łagodniejszy: **L=1**, **M=1.5**, **H=2.5**
- Maksimum: `5*1 + 18*1.5 + 7*2.5 = 49.5`
- **Próg zaliczenia:** `≥ 30 pkt` (≈60% z 49.5)

| Grupa | Poprawnych | Waga | Punkty = Poprawnych × Waga |
|------:|-----------:|-----:|---------------------------:|
| L     |            | 1.0  |                            |
| M     |            | 1.5  |                            |
| H     |            | 2.5  |                            |
| **Suma** |        |      |                            |

**Wzór:**  
`Wynik = L*1.0 + M*1.5 + H*2.5`  
**Werdykt:**  
`zdane` jeśli `Wynik ≥ 30`


## Przykład 1 (zdane)
- L=4, M=9, H=2 → `4*1 + 9*2 + 2*3 = 4 + 18 + 6 = 28` **(to jeszcze nie 37!)**  
- Zmień o 5 pkt: L=4, M=12, H=2 → `4 + 24 + 6 = 34` **(wciąż brak)**  
- L=4, M=12, H=3 → `4 + 24 + 9 = 37` → **ZDANE** ✅

## Przykład 2 (niezdane)
- L=5, M=8, H=1 → `5 + 16 + 3 = 24` → **NIEZDANE** ❌

---

## Szybka ściąga
- **Wariant A (domyślny):** `W = L + 2M + 3H` → **próg 37**  
- **Wariant B (łagodny):** `W = L + 1.5M + 2.5H` → **próg 30**


