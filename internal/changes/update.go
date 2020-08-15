package changes

import "time"

func Update(filename string, hash string) bool {

	if ch, ok := changes[filename]; ok {
		if ch.Hash != hash {
			ch.Hash = hash
			ch.Modified = time.Now().Unix()
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
