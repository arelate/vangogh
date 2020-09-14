package storage

import (
	"encoding/json"
	"io/ioutil"
)

func Load(filename string, data interface{}) error {
	bytes, err := ioutil.ReadFile(filename)
	if err != nil {
		return err
	}

	err = json.Unmarshal(bytes, data)
	if err != nil {
		return err
	}

	return nil
}
