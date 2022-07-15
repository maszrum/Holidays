CREATE TABLE holidays.price_history
(
    id              uuid      NOT NULL,
    offer_id        uuid      NOT NULL,
    price_timestamp timestamp NOT NULL,
    price           integer   NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT offer_id_fkey FOREIGN KEY (offer_id)
        REFERENCES holidays.offer (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);
