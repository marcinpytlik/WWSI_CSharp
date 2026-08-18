# Demo 24 — Outbox + kolejka

Zapis zamówienia i publikacja na szynę są **rozdzielone**: najpierw wiersz w outbox,
potem worker (`FlushAsync`) publikuje. Gdy bus padnie, komunikat zostaje i można ponowić.

Na sali podmień `IMessageBus` na `RabbitPublisher` z demo 11 — testy nie wymagają Dockera.
