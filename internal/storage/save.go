package storage

import (
	"encoding/json"
	"io/ioutil"
	"os"
	"path"
)

func Write(data interface{}, filename string) error {

	bytes, err := json.Marshal(data)
	if err != nil {
		return err
	}

	dir := path.Dir(filename)
	if _, err := os.Stat(dir); os.IsNotExist(err) {
		os.MkdirAll(dir, 0755)
	}

	return ioutil.WriteFile(filename, bytes, 0644)

	return nil
}
