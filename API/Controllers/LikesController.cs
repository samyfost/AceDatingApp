
using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;
    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _likesRepository = likesRepository;
        _userRepository = userRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        var sourceUserId = currentUser.Id;

        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null) return NotFound();

        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");
        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
        if (userLike != null) return BadRequest("You already like this user");
        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to like user");

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
    {
        var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

        var users = await _likesRepository.GetUserLikes(predicate, currentUser.Id);
        return Ok(users);
    }

}