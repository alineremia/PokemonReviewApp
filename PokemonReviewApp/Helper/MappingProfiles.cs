﻿using AutoMapper;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Helper {
    public class MappingProfiles : Profile {
        public MappingProfiles() {
            CreateMap<Pokemon, PokemonDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<Country, CountryDTO>();
            CreateMap<Owner, OwnerDTO>();
            CreateMap<Review, ReviewDTO>();
            CreateMap<Reviewer, ReviewerDTO>();

            CreateMap<CategoryDTO, Category>();
            CreateMap<CountryDTO, Country>();
            CreateMap<OwnerDTO, Owner>();
            CreateMap<PokemonDTO, Pokemon>();
            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewerDTO, Reviewer>();
        }
    }
}
