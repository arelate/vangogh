package gamedetails

import (
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(gameDetails *GameDetails, id int) error {
	return storage.Save(gameDetails, paths.GameDetails(id))
}
