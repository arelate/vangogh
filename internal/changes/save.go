package changes

import (
	"encoding/json"
	"io/ioutil"
)

func Save() error {
	dataBytes, err := json.Marshal(changes)
	if err != nil {
		return err
	}
	return ioutil.WriteFile(Filename, dataBytes, 0644)
}
