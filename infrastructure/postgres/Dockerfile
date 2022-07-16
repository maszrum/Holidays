FROM postgres:14.4

COPY ./init.sql /docker-entrypoint-initdb.d

RUN chown postgres:postgres /docker-entrypoint-initdb.d/init.sql
