Cześć,

W związku z udostępnieniem nowych wersji biblioteki **Microsoft.Data.SqlClient (7.1.2, 7.0.4 oraz 6.1.8)** chciałbym zwrócić uwagę na zmianę dotyczącą walidacji certyfikatów TLS, która może mieć wpływ na połączenia aplikacji z naszymi instancjami SQL Server.

W nowych wersjach zaostrzono walidację parametru `HostNameInCertificate`. Jeżeli parametr jest jawnie ustawiony, a wskazana nazwa nie występuje w certyfikacie serwera, połączenie może zostać odrzucone, nawet jeżeli nazwa serwera podana w connection string jest prawidłowa.

Zmiana ta może być szczególnie istotna w środowiskach wykorzystujących **SQL Server Failover Cluster Instances (FCI)** oraz aliasy DNS.

**Proszę o wykonanie następujących czynności przed aktualizacją biblioteki:**

1. **Inwentaryzacja wersji biblioteki**
   - Sprawdzenie aktualnie wykorzystywanej wersji `Microsoft.Data.SqlClient`.
   - Określenie wersji docelowej oraz aplikacji objętych aktualizacją.

2. **Weryfikacja connection strings**
   - Sprawdzenie parametrów `Encrypt`, `TrustServerCertificate` oraz `HostNameInCertificate`.
   - Weryfikacja zgodności nazw serwerów i aliasów DNS z certyfikatami TLS.
   - Uwzględnienie konfiguracji przechowywanej w plikach, zmiennych środowiskowych oraz systemach zarządzania sekretami.

3. **Testy na środowisku nieprodukcyjnym**
   - Weryfikacja nawiązywania nowych połączeń do SQL Server.
   - Sprawdzenie działania aplikacji po przełączeniu właściciela klastra FCI, jeżeli aplikacja korzysta z takiego środowiska.
   - Kontrola błędów TLS, uwierzytelniania oraz mechanizmu connection pooling.

4. **Weryfikacja po wdrożeniu**
   - Monitorowanie błędów połączeń w logach aplikacyjnych.
   - Kontrola liczby nieudanych prób nawiązania połączeń.
   - Potwierdzenie poprawności działania aplikacji z produkcyjnymi instancjami SQL Server.

**Ważne:** Nie rekomenduję stosowania `TrustServerCertificate=True` jako obejścia problemów z walidacją certyfikatów. W przypadku wystąpienia niezgodności należy zweryfikować konfigurację TLS i usunąć jej przyczynę.

Proszę również o informację, **które aplikacje korzystają z Microsoft.Data.SqlClient, jakie wersje są obecnie wykorzystywane oraz czy planowane są ich aktualizacje**.

Chciałbym, abyśmy wspólnie zweryfikowali potencjalny wpływ tych zmian na środowisko SQL Server przed wdrożeniem produkcyjnym.

Informacje o aktualizacji:
https://lnkd.in/ggGaW2V9

