﻿using AutoMapper;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System.Diagnostics.Metrics;

namespace PokemonReviewApp.Repository {
    public class OwnerRepositor : IOwnerRepository {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public OwnerRepositor(DataContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }
        public ICollection<Owner> GetOwners() {
            return _context.Owners.ToList();
        }

        public Owner GetOwner(int ownerId) {
            return _context.Owners.FirstOrDefault(o => o.Id == ownerId);
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokemonId) {
            return _context.PokemonOwners.Where(o => o.Pokemon.Id == pokemonId).Select(o => o.Owner).ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId) {
            return _context.PokemonOwners.Where(p => p.Owner.Id == ownerId).Select(p => p.Pokemon).ToList();
        }

        public bool OwnerExists(int ownerId) {
            return _context.Owners.Any(o => o.Id == ownerId);
        }

        public bool CreateOwner(Owner owner) {
            _context.Add(owner);
            return Save();
        }

        public bool UpdateOwner(Owner owner) {
           _context.Update(owner);
           return Save();
        }

        public bool DeleteOwner(Owner owner) {
            _context.Remove(owner);
            return Save();
        }

        public bool Save() {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
