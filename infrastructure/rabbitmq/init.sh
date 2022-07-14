#!/bin/sh

production_vhost=production
production_user=holidays
production_password=payAlot
test_vhost=tests
test_user=holidays-test
test_password=payAlot

( rabbitmqctl wait --timeout 60 $RABBITMQ_PID_FILE ; \
rabbitmqctl add_vhost $production_vhost ; \
rabbitmqctl add_user $production_user $production_password ; \
rabbitmqctl set_user_tags $production_user management ; \
rabbitmqctl set_permissions -p $production_vhost $production_user  ".*" ".*" ".*" ; \
rabbitmqctl add_vhost $test_vhost ; \
rabbitmqctl add_user $test_user $test_password ; \
rabbitmqctl set_user_tags $test_user management ; \
rabbitmqctl set_permissions -p $test_vhost $test_user ".*" ".*" ".*" ) &

rabbitmq-server $@