CREATE DATABASE IF NOT EXISTS orleansbank;

USE orleansbank;

CREATE TABLE IF NOT EXISTS tb_account (
    account_id INT PRIMARY KEY,
    balance DOUBLE NOT NULL,
    etag INT NOT NULL,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS tb_transactions (
    transaction_id VARCHAR(32),
    account_id INT,
    amount DOUBLE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (transaction_id, account_id),
    FOREIGN KEY (account_id) REFERENCES tb_account(account_id)
);

CREATE TABLE IF NOT EXISTS tb_idempotency_keys (
    idempotency_key VARCHAR(32),
    account_id INT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (idempotency_key, account_id),
    FOREIGN KEY (account_id) REFERENCES tb_account(account_id)
);

INSERT IGNORE INTO tb_account (account_id, balance, etag)
	VALUES (1, 100.00, 1);
    
INSERT IGNORE INTO tb_account (account_id, balance, etag)
	VALUES (2, 100.00, 1);