CREATE TABLE tache (
    id BIGSERIAL PRIMARY KEY,
    titre VARCHAR(255) NOT NULL,
    faite BOOLEAN NOT NULL DEFAULT FALSE
);

INSERT INTO tache (titre, faite) VALUES
    ('Écrire le Dockerfile multi-stage', false),
    ('Cloisonner les réseaux', false),
    ('Faire passer la CI au vert', false);
