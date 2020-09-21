// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package ids

import (
	"errors"
	"fmt"
	"strconv"
)

func Parse(args []string) ([]int, error) {
	ids := make([]int, 0)
	invalidIds := make([]string, 0)
	for _, arg := range args {
		id, err := strconv.Atoi(arg)
		if err != nil {
			invalidIds = append(invalidIds, arg)
		} else {
			ids = append(ids, id)
		}
	}
	if len(invalidIds) > 0 {
		return ids, errors.New(fmt.Sprintf("NOTE: Invalid ids format or value: %v", invalidIds))
	}

	return ids, nil
}
