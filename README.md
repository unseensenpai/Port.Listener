# **Port Listener**

## It uses PostgreSQL Database, Hangfire Recurring Job, DevExpress XPO ORM, SMTP Mail and .NET's ping library.

### A system that pings the IPs defined in the database and sends an e-mail every 10 minutes for failed/unreachable IPs.

## Set up the database required for the program

#### Create Db
```
psql

CREATE DATABASE IpDb;
```
#### Create Table
```
CREATE TABLE public.tbl_ipchart (
    id serial4 NOT NULL,
    ipaddress varchar(15) NOT NULL,
    branch_id int4 NULL,
    error_count int4 NULL,
    total_errors int4 NULL,
    last_trigger_time timestamp NOT NULL DEFAULT '0001-01-01 00:00:00'::timestamp without time zone,
    is_active bool NOT NULL DEFAULT false,
    CONSTRAINT tbl_ipchart_pkey PRIMARY KEY (id)
);
```

#### Add the auto-incrementing id column sequence for Devexpress XPO to process
```
CREATE SEQUENCE public.tbl_ipchart_id_seq
    INCREMENT BY 1
    MINVALUE 1
    MAXVALUE 2147483647
    START 1
    CACHE 1
    NO CYCLE;
```
### **Project Root: https://github.com/unseensenpai/PortListener**

### All rights reserved. Unseen Software. Said Gülmez Computer Engineer.
