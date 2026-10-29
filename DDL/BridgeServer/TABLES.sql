CREATE TABLE document (
	documentID int PRIMARY KEY,
	name varchar (24) NOT NULL,
	description text,
	extension varchar (24),
	mimetype varchar (24),
	created_at timestamp NOT NULL,
	deleted_at timestamp,
	updated_at timestamp
);

CREATE TABLE rights (
	rightsID int PRIMARY KEY,
	can_read bool NOT NULL,
	can_share bool NOT NULL,
	can_create_version bool NOT NULL,
	can_revoke_version bool NOT NULL,
	can_attest_hash bool NOT NULL,
	can_add_hash_algorithm bool NOT NULL,
	can_revoke_hash bool NOT NULL
);

CREATE TABLE group_document_link (
	documentID int REFERENCES document,
	groupID int PRIMARY KEY,
	rightsID int REFERENCES rights,
	granted_by timestamp NOT NULL, -- Я предполагаю что под "by" имеется ввиду время, а не имя, но я могу ошибаться
	created_at timestamp NOT NULL
);

CREATE TABLE user_document_link (
	documentID int REFERENCES document,
	userID int PRIMARY KEY,
	rightsID int REFERENCES rights,
	is_owner bool NOT NULL,
	granted_by timestamp NOT NULL,
	created_at timestamp NOT NULL
);

CREATE TABLE version_of_document (
	versionID int PRIMARY KEY,
	documentID int REFERENCES document,
	note_for_version varchar (256),
	authorID int NOT NULL,
	version varchar (24) NOT NULL,
	key_for_storage varchar (256) NOT NULL,
	size int NOT NULL,
	created_at timestamp NOT NULL,
	revoked_by timestamp NOT NULL,
	revoked_at timestamp,
	revoked_reason varchar (64) 
);

CREATE TABLE public_key (
	public_keyID int PRIMARY KEY,
	owner_id int NOT NULL,
	key varchar (256) NOT NULL,
	created_at timestamp NOT NULL,
	algorithm varchar (24) NOT NULL,
	attestation bool NOT NULL,
	revoked_by timestamp NOT NULL,
	revoked_at timestamp,
	revoked_reason varchar (64)
);

CREATE TABLE hash_of_version (
	hashID int PRIMARY KEY,
	versionID int REFERENCES version_of_document,
	algorithm varchar (24) NOT NULL,
	authorID int NOT NULL,
	hashbytes bytea NOT NULL,
	created_at timestamp NOT NULL,
	revoked_by timestamp NOT NULL,
	revoked_at timestamp,
	revoked_reason varchar (64),
	is_primary bool NOT NULL
);

CREATE TABLE signature (
	signatureID int PRIMARY KEY,
	versionID int REFERENCES version_of_document,
	hashID int REFERENCES hash_of_version,
	public_keyID int REFERENCES public_key,
	authorID int NOT NULL,
	signature_bytes bytea NOT NULL,
	signed_at timestamp NOT NULL,
	revoked_by timestamp NOT NULL,
	revoked_at timestamp,
	revoked_reason varchar (64)
);

CREATE INDEX document_id_index ON document (documentID);
CREATE INDEX document_name_index ON document (name);

CREATE INDEX version_id_index ON version_of_document (versionID);
CREATE INDEX version_name_index ON version_of_document (version);