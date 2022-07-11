CREATE TABLE holidays.offer_event_log
(
    id uuid NOT NULL,
    offer_id uuid NOT NULL,
    event_type character varying(64),
    params character varying(256),
    PRIMARY KEY (id),
    CONSTRAINT offer_id_fkey FOREIGN KEY (offer_id)
        REFERENCES holidays.offer (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);