package gamedetails

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(gameDetails GameDetails, id int) error {
	return storage.Save(gameDetails, paths.GameDetails(id))
}

func Load(id int) (gameDetails *GameDetails, err error) {
	gdBytes, err := storage.Load(paths.GameDetails(id))

	if err != nil {
		return gameDetails, err
	}

	err = json.Unmarshal(gdBytes, &gameDetails)

	return gameDetails, err
}
