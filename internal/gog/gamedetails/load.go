package gamedetails

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int) (gameDetails *GameDetails, err error) {
	gdBytes, err := storage.Load(paths.GameDetails(id))

	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(gdBytes, &gameDetails)

	return gameDetails, err
}
