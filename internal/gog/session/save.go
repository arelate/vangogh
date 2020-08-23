package session

import (
	"github.com/boggydigital/vangogh/internal/storage"
	"net/http"
)

const cookiesFilename = "cookies.json"

func Save(cookies []*http.Cookie) error {
	return storage.Write(cookies, cookiesFilename)
}
