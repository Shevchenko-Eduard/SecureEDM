CREATE SCHEMA Bridge
	CREATE TABLE document (
		documentID int PRIMARY KEY,
		name varchar (24) NOT NULL,
		description text,
		extension varchar (24) NOT NULL,
		mimetype varchar (256) NOT NULL,
		created_at timestamp NOT NULL,
		deleted_at timestamp,
		updated_at timestamp
	)
	
	CREATE TABLE rights (
		rightsID int PRIMARY KEY,
		can_read bool NOT NULL,
		can_share bool NOT NULL,
		can_create_version bool NOT NULL,
		can_revoke_version bool NOT NULL,
		can_attest_hash bool NOT NULL,
		can_add_hash_algorithm bool NOT NULL,
		can_revoke_hash bool NOT NULL
	)
	
	CREATE TABLE group_document_link (
		documentID int REFERENCES document,
		groupID int,
		rightsID int REFERENCES rights,
		granted_by int NOT NULL, -- Я предполагаю что под "by" имеется ввиду время, а не имя, но я могу ошибаться
		created_at timestamp NOT NULL,
		
		CONSTRAINT pk_documentid_groupid_lnk PRIMARY KEY (documentID, groupID)
	)
	
	CREATE TABLE user_document_link (
		documentID int REFERENCES document,
		userID int,
		rightsID int REFERENCES rights,
		is_owner bool NOT NULL,
		granted_by int NOT NULL,
		created_at timestamp NOT NULL,
		
		CONSTRAINT pk_documentid_userid_lnk PRIMARY KEY (documentID, userID)
	)
	
	CREATE TABLE version_of_document (
		versionID int PRIMARY KEY,
		documentID int REFERENCES document,
		note text,
		authorID int NOT NULL,
		name varchar (24) NOT NULL,
		key_for_storage bytea NOT NULL,
		size int NOT NULL,
		created_at timestamp NOT NULL,
		revoked_by int,
		revoked_at timestamp,
		revoked_reason varchar (64) 
	)
	
	CREATE TABLE public_key (
		public_keyID int PRIMARY KEY,
		ownerID int NOT NULL,
		key_value bytea NOT NULL,
		created_at timestamp NOT NULL,
		keygen_algorithm varchar (24) NOT NULL,
		attestation bytea NOT NULL,
		revoked_by int,
		revoked_at timestamp,
		revoked_reason varchar (64)
	)
	
	CREATE TABLE hash_of_version (
		hashID int PRIMARY KEY,
		versionID int REFERENCES version_of_document,
		hashed_algorithm varchar (24) NOT NULL,
		authorID int NOT NULL,
		hash_value bytea NOT NULL,
		created_at timestamp NOT NULL,
		revoked_by int,
		revoked_at timestamp,
		revoked_reason varchar (64),
		is_primary bool NOT NULL
	)
	
	CREATE TABLE signature (
		signatureID int PRIMARY KEY,
		versionID int REFERENCES version_of_document,
		public_keyID int REFERENCES public_key,
		authorID int NOT NULL,
		signature_value bytea NOT NULL,
		signed_algorithm varchar (24) NOT NULL,
		signed_at timestamp NOT NULL,
		revoked_by int,
		revoked_at timestamp,
		revoked_reason varchar (64)
	)
	
	CREATE INDEX document_name_index ON document (name)
	
	CREATE INDEX version_document_index ON version_of_document (documentID)
	CREATE INDEX version_author_index ON version_of_document (authorID)
	CREATE INDEX version_name_index ON version_of_document (name)
	
	CREATE INDEX publickey_owner_index ON public_key (ownerID)
	
	CREATE INDEX hash_version_index ON hash_of_version (versionID)
	CREATE INDEX hash_author_index ON hash_of_version (authorID)
	
	CREATE INDEX signature_verison_index ON signature (versionID)
	CREATE INDEX signature_publickey_index ON signature (public_keyID)
	CREATE INDEX signature_author_index ON signature (authorID);
	
	
	