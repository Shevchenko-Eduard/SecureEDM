CREATE SCHEMA Auth
	CREATE TABLE user_setting (
		settingID int PRIMARY KEY,
		expires_of_session_in interval NOT NULL,
		expires_of_trefresh_token_in interval NOT NULL,
		expires_of_taccess_token_in interval NOT NULL,
		expires_of_attestation_noise interval NOT NULL
	)
	
	CREATE TABLE "user" (
		userID int PRIMARY KEY, 
		name varchar (64) NOT NULL,
		email varchar (64), 
		phone varchar (24),
		DOB date,
		created_at timestamp NOT NULL,
		settingID int REFERENCES user_setting (settingID),
		updated_at timestamp,
		deleted_at timestamp
	)
	
	CREATE TABLE attestation (
		attestationID int PRIMARY KEY,
		userID int REFERENCES "user",
		public_key bytea NOT NULL, -- Размер хэшей и ключей не знаю
		noise bytea NOT NULL,
		signature_algorithm bytea, 
		signature bytea,
		Created_at timestamp NOT NULL,
		attest bytea,
		revoked_at timestamp,
		attestation_at timestamp,
		revoked_reason varchar (64)
	)
	
	CREATE TABLE hash_of_password (
		hashID int PRIMARY KEY,
		userID int REFERENCES "user",
		hash_value bytea NOT NULL,
		hashed_algorithm bytea NOT NULL,
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64)
	)
	
	CREATE TABLE "group" (
		groupID int PRIMARY KEY,
		ownerID int REFERENCES "user" (userID),
		name varchar (64) NOT NULL,
		description text,
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64)
	)
	
	CREATE TABLE user_group_link (
		userID int REFERENCES "user",
		groupID int REFERENCES "group",
		is_admin bool NOT NULL,
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64),
		
		CONSTRAINT pk_userid_groupid_lnk PRIMARY KEY (userID, groupID)
	)
	
	CREATE TABLE parent_group (
		parent_groupID int REFERENCES "group" (groupID),
		sub_groupID int REFERENCES "group" (groupID),
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64),
		
		CONSTRAINT pk_parentgroupid_subgroupid PRIMARY KEY (parent_groupID, sub_groupID)
	)
	
	CREATE TABLE session (
		sessionID int PRIMARY KEY,
		userID int REFERENCES "user",
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64),
		last_extension timestamp NOT NULL
	)
	
	CREATE TABLE refresh_token (
		tokenID int PRIMARY KEY,
		sessionID int REFERENCES session,
		hash_of_token_bytes bytea NOT NULL,
		algorithm varchar (64) NOT NULL,
		created_at timestamp NOT NULL,
		revoked_at timestamp,
		revoked_reason varchar (64),
		last_extension timestamp -- Не уверен что это дата
	)
	
	CREATE INDEX user_email_index ON "user" (email)
	CREATE INDEX user_name_index ON "user" (name)
	
	CREATE INDEX group_owner_index ON "group" (ownerID)
	CREATE INDEX group_name_index ON "group" (name)
	
	CREATE INDEX session_extension_index ON "session" (last_extension)
	CREATE INDEX session_user_index ON "session" (userID)
	
	CREATE INDEX refreshtk_extension_index ON "refresh_token" (last_extension);
