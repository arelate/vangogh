// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package schema

type Product struct {
	Developer string   `json:"developer"`
	Publisher string   `json:"publisher"`
	Gallery   []string `json:"gallery"`
	Video     struct {
		ID       string `json:"id"`
		Provider string `json:"provider"`
	} `json:"video"`
	SupportedOperatingSystems []string `json:"supportedOperatingSystems"`
	Genres                    []string `json:"genres"`
	GlobalReleaseDate         int      `json:"globalReleaseDate"`
	IsTBA                     bool     `json:"isTBA"`
	Price                     struct {
		Amount                     string      `json:"amount"`
		BaseAmount                 string      `json:"baseAmount"`
		FinalAmount                string      `json:"finalAmount"`
		IsDiscounted               bool        `json:"isDiscounted"`
		DiscountPercentage         int         `json:"discountPercentage"`
		DiscountDifference         string      `json:"discountDifference"`
		Symbol                     string      `json:"symbol"`
		IsFree                     bool        `json:"isFree"`
		Discount                   int         `json:"discount"`
		IsBonusStoreCreditIncluded bool        `json:"isBonusStoreCreditIncluded"`
		BonusStoreCreditAmount     string      `json:"bonusStoreCreditAmount"`
		PromoID                    interface{} `json:"promoId"`
	} `json:"price"`
	IsDiscounted    bool `json:"isDiscounted"`
	IsInDevelopment bool `json:"isInDevelopment"`
	ID              int  `json:"id" bson:"_id"`
	ReleaseDate     int  `json:"releaseDate"`
	Availability    struct {
		IsAvailable          bool `json:"isAvailable"`
		IsAvailableInAccount bool `json:"isAvailableInAccount"`
	} `json:"availability"`
	SalesVisibility struct {
		IsActive   bool `json:"isActive"`
		FromObject struct {
			Date         string `json:"date"`
			TimezoneType int    `json:"timezone_type"`
			Timezone     string `json:"timezone"`
		} `json:"fromObject"`
		From     int `json:"from"`
		ToObject struct {
			Date         string `json:"date"`
			TimezoneType int    `json:"timezone_type"`
			Timezone     string `json:"timezone"`
		} `json:"toObject"`
		To int `json:"to"`
	} `json:"salesVisibility"`
	Buyable    bool   `json:"buyable"`
	Title      string `json:"title"`
	Image      string `json:"image"`
	URL        string `json:"url"`
	SupportURL string `json:"supportUrl"`
	ForumURL   string `json:"forumUrl"`
	WorksOn    struct {
		Windows bool `json:"Windows"`
		Mac     bool `json:"Mac"`
		Linux   bool `json:"Linux"`
	} `json:"worksOn"`
	Category         string `json:"category"`
	OriginalCategory string `json:"originalCategory"`
	Rating           int    `json:"rating"`
	Type             int    `json:"type"`
	IsComingSoon     bool   `json:"isComingSoon"`
	Slug             string `json:"slug"`
}

// NOTE: Not marshalling the following fields:
// CustomAttributes []interface{} `json:"customAttributes"`
// IsWishlistable   bool   `json:"isWishlistable"`
// IsPriceVisible   bool   `json:"isPriceVisible"`
// IsMovie          bool   `json:"isMovie"`
// IsGame           bool   `json:"isGame"`
