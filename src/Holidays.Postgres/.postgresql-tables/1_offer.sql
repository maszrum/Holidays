CREATE TABLE holidays.offer
(
    id uuid NOT NULL,
    hotel character varying(128) NOT NULL,
    destination character varying(128) NOT NULL,
    departure_date integer NOT NULL,
    days integer NOT NULL,
    city_of_departure character varying(128) NOT NULL,
    price integer NOT NULL,
    details_url character varying(1024) NOT NULL,
    website_name character varying(32) NOT NULL,
    is_removed boolean NOT NULL,
    PRIMARY KEY (id)
);