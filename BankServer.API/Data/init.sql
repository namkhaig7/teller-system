-- Runs once, automatically, the first time the postgres container starts with an empty
-- data volume (that's how the official postgres Docker image's docker-entrypoint-initdb.d
-- mechanism works -- no EF migrations needed for a schema this small).

CREATE TABLE accounts (
    account_number  TEXT PRIMARY KEY,
    owner_name      TEXT NOT NULL,
    balance         NUMERIC(18, 2) NOT NULL,
    currency_code   TEXT NOT NULL
);

CREATE TABLE tickets (
    number          SERIAL PRIMARY KEY,
    issued_at_utc   TIMESTAMPTZ NOT NULL,
    status          TEXT NOT NULL,       -- 'Waiting' | 'Called'
    counter_number  INT NULL,
    called_at_utc   TIMESTAMPTZ NULL
);

CREATE TABLE exchange_rates (
    currency_code   TEXT PRIMARY KEY,
    buy_rate        NUMERIC(18, 4) NOT NULL,
    sell_rate       NUMERIC(18, 4) NOT NULL,
    updated_at_utc  TIMESTAMPTZ NOT NULL
);

-- Seed data so the apps have something to show before anyone's touched them.
INSERT INTO accounts (account_number, owner_name, balance, currency_code) VALUES
    ('1000000001', 'Bat',  500000.00, 'MNT'),
    ('1000000002', 'Bold', 250000.00, 'MNT');

INSERT INTO exchange_rates (currency_code, buy_rate, sell_rate, updated_at_utc) VALUES
    ('USD', 3450.00, 3480.00, now()),
    ('EUR', 3700.00, 3740.00, now());
