CREATE TABLE user_setting (
	settingID int PRIMARY KEY,
	expires_of_session_in interval NOT NULL,
	expires_of_trefresh_token_in interval NOT NULL,
	expires_of_taccess_token_in interval NOT NULL,
	expires_of_attestation_noise interval NOT NULL
);

CREATE TABLE "user" (
	userID int PRIMARY KEY, 
	name varchar (64) NOT NULL,
	email varchar (64), 
	phone varchar (24),
	DOB date,
	created_at timestamp NOT NULL,
	settingID int REFERENCES user_setting (settingID),
	updated_at timestamp NOT NULL,
	deleted_at timestamp NOT NULL
);

CREATE TABLE attestation (
	attestationID int PRIMARY KEY,
	userID int REFERENCES "user",
	public_key varchar (256) NOT NULL, -- Размер хэшей и ключей не знаю
	noise varchar (256) NOT NULL,
	signature_algorithm varchar (256) NOT NULL, 
	hashed_algorithm varchar (256) NOT NULL,
	signature varchar (256) NOT NULL,
	Created_at timestamp NOT NULL,
	attest bool NOT NULL,
	revoked_at timestamp NOT NULL,
	attestation_at timestamp NOT NULL
);

CREATE TABLE hash_of_password (
	hashID int PRIMARY KEY,
	userID int REFERENCES "user",
	hash_bytes bytea NOT NULL,
	algorithm varchar (256) NOT NULL,
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL
);

CREATE TABLE "group" (
	groupID int PRIMARY KEY,
	ownerID int REFERENCES "user" (userID),
	name varchar (64) NOT NULL,
	description text,
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL
);

CREATE TABLE user_group_link (
	userID int REFERENCES "user",
	groupID int REFERENCES "group",
	is_admin bool NOT NULL,
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL
);

CREATE TABLE parent_group (
	parent_groupID int REFERENCES "group" (groupID),
	sub_groupID int REFERENCES "group" (groupID),
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL
);

CREATE TABLE session (
	sessionID int PRIMARY KEY,
	userID int REFERENCES "user",
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL,
	last_extension timestamp NOT NULL
);

CREATE TABLE refresh_token (
	tokenID int PRIMARY KEY,
	sessionID int REFERENCES session,
	hash_of_token_bytes bytea NOT NULL,
	algorithm varchar (64) NOT NULL,
	created_at timestamp NOT NULL,
	revoked_at timestamp NOT NULL,
	last_extension timestamp NOT NULL -- Не уверен что это дата
);

CREATE INDEX user_id_index ON "user" (userID);
CREATE INDEX user_name_index ON "user" (name);

CREATE INDEX group_id_index ON "group" (groupID);
CREATE INDEX group_name_index ON "group" (name);
