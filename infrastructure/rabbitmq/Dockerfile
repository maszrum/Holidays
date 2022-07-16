FROM rabbitmq:3.10.6-management

ENV RABBITMQ_PID_FILE /var/lib/rabbitmq/mnesia/rabbitmq

ADD ./init.sh /init.sh
RUN chmod +x /init.sh

EXPOSE 15672
EXPOSE 5672

CMD ["/init.sh"]
