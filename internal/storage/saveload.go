package storage

import (
	"encoding/json"
	"io/ioutil"
	"os"
)

func Save(data interface{}, filename string) error {
	dataBytes, err := json.Marshal(data)
	if err != nil {
		return err
	}
	return ioutil.WriteFile(filename, dataBytes, 0644)
}

func Load(filename string) (data []byte, err error) {
	data = nil
	if _, e := os.Stat(filename); e == nil {

		data, err = ioutil.ReadFile(filename)
		if err != nil {
			return data, err
		}
	}
	return data, nil
}
