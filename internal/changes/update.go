package changes

import (
	"github.com/boggydigital/vangogh/internal/filenames"
	"time"
)

func Update(filename string, hash string) bool {

	if filename == filenames.Cookies {
		return true
	}

	if ch, ok := changes[filename]; ok {
		if ch.Hash != hash {
			ch.Hash = hash
			ch.Modified = time.Now().Unix()
			changes[filename] = ch
			return true
		}
		return false
	} else {
		changes[filename] = Change{
			Hash:     hash,
			Created:  time.Now().Unix(),
			Modified: time.Now().Unix(),
		}
		return true
	}
}
