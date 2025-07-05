using BaseProject.Model.Entities;
using BaseProject.Repository.FileManagerRepository;
using BaseProject.Service.Common.Service;
using BaseProject.Service.FileManagerService.Dto;
using BaseProject.Service.Common;
using BaseProject.Service.Dto;
using Microsoft.EntityFrameworkCore;
using BaseProject.Service.Core.Mapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using OfficeOpenXml.Drawing.Chart;
using Microsoft.AspNetCore.Identity;
using BaseProject.Repository.FileSecurityRepository;
using BaseProject.Service.Constant;
using BaseProject.Repository.UserRoleRepository;
using System.Security.AccessControl;
using System.IO.Pipes;
using System.Security.Policy;
using System.ComponentModel;
using BaseProject.Service.FileSecurityService.Dto;
using BaseProject.Repository.AppUserRepository;
using BaseProject.Repository.AspNetUsersRepository;
using BaseProject.Repository.RoleRepository;
using BaseProject.Repository.DepartmentRepository;
using Microsoft.EntityFrameworkCore.Storage;
using System.IO.Compression;
using Org.BouncyCastle.Asn1.Ocsp;
using BaseProject.Model.Entities;
using Service.Common;



namespace BaseProject.Service.FileManagerService
{
    public class FileManagerService : Service<FileManager>, IFileManagerService
    {

        private readonly IFileSecurityRepository _fileSecurityRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IAspNetUsersRepository _aspNetUsersRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public FileManagerService(
            IFileManagerRepository fileManagerRepository
            , IFileSecurityRepository fileSecurityRepository,
            IUserRoleRepository userRoleRepository,
            IAspNetUsersRepository aspNetUsersRepository,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository) : base(fileManagerRepository)
        {
            _fileSecurityRepository = fileSecurityRepository;
            _userRoleRepository = userRoleRepository;
            _aspNetUsersRepository = aspNetUsersRepository;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
        }


        #region Những hàm dùng chung

        /// <summary>
        /// Lấy file/folder con để tải xuống zip
        /// </summary>
        /// <param name="id">id folder cần lấy con</param>
        /// <param name="allFiles">tất cả các file/folder trong db</param>
        /// <param name="removeHeaderParent">Xóa đoạn path đầu cần xóa để tránh nén thừa folder cha</param>
        /// <returns>list file/folder con</returns>
        public async Task<List<FileManager>> GetChildToZip(Guid id, List<FileManager> allFiles, string removeHeaderParent)
        {
            var result = new List<FileManager>();
            if (id == new Guid()) return result;
            if (!allFiles.Any()) return result;

            var childs = allFiles.Where(x => x.ParentId == id).ToList();
            if (childs != null && childs.Any())
            {
                foreach (var item in childs)
                {
                    if (!string.IsNullOrEmpty(removeHeaderParent) &&
                   !string.IsNullOrEmpty(item.Path) &&
                   item.Path.StartsWith(removeHeaderParent))
                    {
                        item.Path = item.Path.Substring(removeHeaderParent.Length).TrimStart('/');
                    }
                    result.AddRange(await GetChildToZip(item.Id, allFiles, removeHeaderParent));
                }
                result.AddRange(childs);
            }
            return result;
        }

        /// <summary>
        /// Lấy các file/folder con
        /// </summary>
        /// <param name="ids">id những folder cần lấy con</param>
        /// <param name="allFiles">tất cả file/folder trong db</param>
        /// <returns>Tất cả file/folder con</returns>
        public async Task<List<FileManager>> GetChilds(List<Guid>? ids, List<FileManager> allFiles)
        {
            var result = new List<FileManager>();
            if (ids == null || !ids.Any()) return result;
            if (!allFiles.Any()) return result;

            var childs = allFiles
                .Where(x => x.ParentId != null && ids.Contains((Guid)x.ParentId))
                .ToList();
            if (childs != null && childs.Any())
            {
                result.AddRange(childs);
                result.AddRange(await GetChilds(childs.Select(x => x.Id).ToList(), allFiles));
            }
            return result;
        }

        /// <summary>
        /// Lấy đường dẫn mới khi thay đổi tên
        /// </summary>
        /// <param name="parentID">ID thư mục cha</param>
        /// <param name="name">Tên mới</param>
        /// <returns>Đường dẫn mới</returns>
        public async Task<string> GetPath(Guid? parentID, string name)
        {
            string folderPath = "";
            if (parentID != null)
            {
                var parentFolder = FindBy(x => x.Id == parentID).FirstOrDefault();
                if (parentFolder != null)
                {
                    folderPath = parentFolder.Path + "/" + name;
                }
                else
                {
                    folderPath = "/" + name;
                }
            }
            else
            {
                folderPath = "/" + name;
            }

            return folderPath;
        }

        /// <summary>
        /// Tạo tên file ko bị trùng trong db
        /// </summary>
        /// <param name="parentID">ID thư mục cha</param>
        /// <param name="baseName">Tên hiện tại</param>
        /// <returns>Tên mới hợp lệ</returns>
        public async Task<string> GenerateUniqueNameFromDB(Guid? parentID, string baseName)
        {
            string nameWithoutSuffix = baseName;
            string extension = "";
            int i = 1;

            // Nếu có phần mở rộng (ví dụ: .txt), tách riêng
            if (Path.HasExtension(baseName))
            {
                extension = Path.GetExtension(baseName);
                nameWithoutSuffix = Path.GetFileNameWithoutExtension(baseName);
            }

            string newName = baseName;

            while (await ExistsName(parentID, newName))
            {
                newName = $"{nameWithoutSuffix} ({i}){extension}";
                i++;
            }

            return newName;
        }

        /// <summary>
        /// Tạo tên file vật lý hợp lệ
        /// </summary>
        /// <param name="directory">thư mục lưu file</param>
        /// <param name="baseName">tên file hiện tại</param>
        /// <returns>tên file mới</returns>
        public string GenerateUniqueNamePhysical(string directory, string baseName)
        {
            string nameWithoutExtension = baseName;
            string extension = "";
            int i = 1;

            // Kiểm tra nếu là file (có phần mở rộng)
            if (Path.HasExtension(baseName))
            {
                extension = Path.GetExtension(baseName);
                nameWithoutExtension = Path.GetFileNameWithoutExtension(baseName);
            }

            string newName = baseName;

            while (File.Exists(Path.Combine(directory, newName)) || Directory.Exists(Path.Combine(directory, newName)))
            {
                newName = $"{nameWithoutExtension} ({i}){extension}";
                i++;
            }

            return newName;
        }

        /// <summary>
        /// Cập nhật đường dẫn của các tài liệu nằm trong khi thư mục thay đổi
        /// </summary>
        /// <param name="file">thư mục cần cập nhật</param>
        /// <returns></returns>
        public async Task UpdatePathChilds(FileManager file)
        {
            var childrens = FindBy(x => x.ParentId == file.Id).ToList();
            if (childrens == null || !childrens.Any())
            {
                return;
            }

            foreach (var item in childrens)
            {
                var oldPath = item.Path;
                item.Path = file.Path + "/" + item.Name;
                await UpdateAsync(item);

                if (item.IsDirectory == true)
                {
                    await UpdatePathChilds(item);
                }
            }
        }

        /// <summary>
        /// Kiểm tra tên tài liệu đã tồn tại chưa
        /// </summary>
        /// <param name="parentId">Id thư mục cha</param>
        /// <param name="itemName">Tên mới</param>
        /// <returns>true nếu tồn tại</returns>
        public async Task<bool> ExistsName(Guid? parentId, string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName)) return true;

            // thư mục cha là thư mục gốc
            if (parentId == null)
            {
                var lstFile = FindBy(x => x.ParentId == null).ToList();
                if (lstFile != null && lstFile.Any())
                {
                    return lstFile.Any(x => x.Name.Trim().ToLower() == itemName.Trim().ToLower());
                }
            }
            else
            {
                var NameExistsed = FindBy(x => x.ParentId == parentId)
                .Select(x => x.Name)
                .ToList();
                if (NameExistsed == null || !NameExistsed.Any()) return false;

                return NameExistsed.Any(x => x.Trim().ToLower().Equals(itemName.Trim().ToLower()));
            }
            return false;
        }


        #endregion

        #region Xóa file or folder
        /// <summary>
        /// Xóa file vật lý
        /// </summary>
        /// <param name="direction">Đường dẫn tới file</param>
        /// <returns></returns>
        public async Task<bool> DeletePhysical(string direction)
        {
            try
            {
                var fullPath = direction.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                //else if (Directory.Exists(fullPath))
                //{
                //    Directory.Delete(fullPath, true); 
                //}
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Xóa file or folder cả db và vật lý
        /// </summary>
        /// <param name="file">file cần xóa</param>
        /// <param name="allFile">tất cả file</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileOrFolder(FileManager file, List<FileManager> allFile, string rootPath)
        {
            try
            {
                if (file == null) return true;
                var childs = allFile.Where(x => x.ParentId == file.Id).ToList();
                if (childs == null && !childs.Any()) return true;
                foreach (var item in childs)
                {
                    await DeleteFileOrFolder(item, allFile, rootPath);
                }
                if (file.IsDirectory != true)
                {
                    var direction = Path.Combine(rootPath, file.PhysicalPath);
                    await DeletePhysical(direction);
                }
                await DeleteAsync(file);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Xóa nhiều file or folder
        /// </summary>
        /// <param name="ids">danh sách id tài liệu cần xóa</param>
        /// <param name="rootPath">thư mục lưu file vật lý</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileorFolders(List<Guid> ids, string rootPath)
        {
            if (ids == null || !ids.Any()) return true;
            var allFile = await GetQueryable().ToListAsync();
            foreach (var item in ids)
            {
                var obj = allFile.FirstOrDefault(x => x.Id == item);
                if (obj != null)
                {
                    await DeleteFileOrFolder(obj, allFile, rootPath);
                }
            }
            return true;
        }

        #endregion Xóa file or folder

        #region Tạo file or folder

        /// <summary>
        /// Tạo folder trong db
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> CreateFolderFromDB(FileManager folder)
        {
            try
            {
                if (folder == null) return false;
                // tạo tên mới nếu trùng
                var name = await GenerateUniqueNameFromDB(folder.ParentId, folder.Name);

                // tạo đường đẫn
                string folderPath = await GetPath(folder.ParentId, folder.Name);

                folder.Path = folderPath;
                folder.Name = name;

                await CreateAsync(folder);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tạos file trong db
        /// </summary>
        /// <param name="parentID">ID thư mục cha</param>
        /// <param name="file">file</param>
        /// <param name="physicalPath">đường dẫn vật lý</param>
        /// <returns></returns>
        public async Task<FileManager> CreateFileFromDB(Guid? parentID, IFormFile file, string physicalPath)
        {
            if (file == null || string.IsNullOrEmpty(physicalPath)) return null;
            try
            {
                // tạo tên hợp lệ
                var newName = await GenerateUniqueNameFromDB(parentID, file.FileName);
                // tạo đường dẫn
                string filePath = await GetPath(parentID, newName);

                var fileManager = new FileManager();
                fileManager.Name = newName;
                fileManager.IsDirectory = false;
                fileManager.Path = filePath;
                fileManager.ParentId = parentID ?? null;
                fileManager.Size = file.Length;
                fileManager.PhysicalPath = physicalPath;

                await CreateAsync(fileManager);
                return fileManager;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Tạo file vật lý
        /// </summary>
        /// <param name="rootPath">Đường dẫn tới thư mục lưu file vật lý</param>
        /// <param name="file">File cần tạo</param>
        /// <returns>Tên file đã lưu</returns>
        public async Task<string> CreatePhysicalFile(string direction, IFormFile file)
        {
            string result = string.Empty;
            try
            {
                if (file == null || file.Length == 0) return result;

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(direction))
                {
                    Directory.CreateDirectory(direction);
                }

                // Đổi tên nếu file trùng
                string filePathToUse = Path.Combine(direction, GenerateUniqueNamePhysical(direction, file.FileName));


                using (var stream = new FileStream(filePathToUse, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result = Path.GetFileName(filePathToUse);   
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }


        /// <summary>
        /// Tạo file cả vật lý và db
        /// </summary>
        /// <param name="rootPath">Đường dẫn thư mục lưu file vật lý</param>
        /// <param name="parentID">Id thư mục cha</param>
        /// <param name="file">file cần tạo</param>
        /// <returns>file db đã tạo</returns>
        public async Task<FileManager?> CreateFile(string rootPath, Guid? parentID, IFormFile file)
        {
            if (string.IsNullOrEmpty(rootPath) || file == null) return null;
            try
            {
                var fileNamePS = await CreatePhysicalFile(rootPath, file);
                if (string.IsNullOrEmpty(fileNamePS)) return null;
                var fileDB = await CreateFileFromDB(parentID, file, fileNamePS);
                if (fileDB != null) return fileDB;
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        #endregion Tạo file or folder


        #region Cập nhật file or folder

        /// <summary>
        /// Đổi tên file hoặc folder
        /// </summary>
        /// <param name="file">tài liệu cần đổi tên</param>
        /// <param name="newName">Tên mới</param>
        /// <returns>Tài liệu đã đổi tên</returns>
        public async Task<FileManager> RenameFileOrFolder(FileManager file, string newName)
        {
            if (file == null || string.IsNullOrWhiteSpace(newName)) return null;

            try
            {
                // tạo tên hợp lệ
                var name = await GenerateUniqueNameFromDB(file.ParentId, newName);
                file.Name = name;

                // tạo path mới
                var path = await GetPath(file.ParentId, name);
                file.Path = path;

                await UpdateAsync(file);

                // cập nhật lại path cho tệp con
                await UpdatePathChilds(file);

                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Copy file vật lý
        /// </summary>
        /// <param name="sourcePath">Đường dẫn tới file cần copy</param>
        /// <returns>Tên file mới</returns>
        public async Task<string> CopyFilePhysical(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath)) return string.Empty;
            try
            {
                string directory = Path.GetDirectoryName(sourcePath)!;
                string originalFileName = Path.GetFileName(sourcePath);

                // Tạo tên file mới không trùng
                string newFileName = GenerateUniqueNamePhysical(directory, originalFileName);
                string newPath = Path.Combine(directory, newFileName);

                // Sao chép file
                File.Copy(sourcePath, newPath, overwrite: false);

                return newFileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Copy file or folder db và vật lý
        /// </summary>
        /// <param name="sourceItem">item cần copy</param>
        /// <param name="destinationFolder">thư mục chứa item copy</param>
        /// <param name="allFile">Danh sách tất cả tài liệu</param>
        /// <param name="rootPath">Đường dẫn file vật lý</param>
        /// <returns></returns>
        public async Task CopyFileOrFolder(FileManager sourceItem, FileManager? destinationFolder, List<FileManager> allFile, string rootPath)
        {
            FileManager fileManager = new FileManager();

            // chuyển đến thư mục mới cần phải kiểm tra tên unique
            fileManager.Name = await GenerateUniqueNameFromDB(destinationFolder?.Id, sourceItem.Name);
            fileManager.IsDirectory = sourceItem.IsDirectory;
            fileManager.Path = (destinationFolder?.Path ?? "") + "/" + fileManager.Name;
            fileManager.ParentId = destinationFolder?.Id ?? null;
            fileManager.Size = sourceItem.Size;
            fileManager.FileExtension = sourceItem.FileExtension;
            fileManager.MimeType = sourceItem.MimeType;

            // copy file vật lý
            if (sourceItem.IsDirectory != true)
            {
                var sourcePath = Path.Combine(rootPath, sourceItem.PhysicalPath);
                fileManager.PhysicalPath = await CopyFilePhysical(sourcePath);
            }

            await CreateAsync(fileManager);

            // nếu là folder thì cần copy các tệp con
            if (sourceItem.IsDirectory == true)
            {
                var childs = allFile.Where(x => x.ParentId == sourceItem.Id).ToList();
                if (childs == null || !childs.Any()) return;

                foreach (var item in childs)
                {
                    await CopyFileOrFolder(item, fileManager, allFile, rootPath);
                }
            }

        }

        /// <summary>
        /// Di chuyển file or folder
        /// </summary>
        /// <param name="sourceItem">item cần di chuyển</param>
        /// <param name="destinationFolder">folder đích</param>
        /// <param name="allFile">tất cả tài liệu</param>
        /// <returns></returns>
        public async Task MoveFileOrFolder(FileManager sourceItem, FileManager? destinationFolder, List<FileManager> allFile)
        {
            // nếu cắt/dán tại chỗ thì ko thực hiện
            if (sourceItem.ParentId == destinationFolder?.Id) return;

            sourceItem.Name = await GenerateUniqueNameFromDB(destinationFolder?.Id, sourceItem.Name);
            sourceItem.Path = (destinationFolder?.Path ?? "") + "/" + sourceItem.Name;
            sourceItem.ParentId = destinationFolder?.Id ?? null;

            await UpdateAsync(sourceItem);

            if (sourceItem.IsDirectory == true)
            {
                await UpdatePathChilds(sourceItem);
            }
        }

        /// <summary>
        /// Lưu thông tin phân quyền/ chia sẻ
        /// </summary>
        /// <param name="fileSecurities">danh sách thông tin phân quyền/ chia sẻ</param>
        /// <param name="fileID">ID file được chia sẻ</param>
        /// <returns></returns>


        /// <summary>
        /// Lấy thông tin phân quyền chia sẻ
        /// </summary>
        /// <param name="fileID">id tài liệu được chia sẻ</param>
        /// <param name="sharedByID">id đối tượng được chia sẻ</param>
        /// <returns></returns>
        public async Task<List<FileSecurityDto>> GetShare(Guid fileID, Guid sharedByID)
        {
            try
            {
                var data = _fileSecurityRepository
                    .FindBy(x => x.FileID == fileID)
                    .Where(x => x.SharedByID == sharedByID)
                    .Select(x => new FileSecurityDto()
                    {
                        SharedByID = x.SharedByID,
                        FileID = x.FileID,
                        SharedToType = x.SharedToType,
                        SharedToID = x.SharedToID,
                        CanRead = x.CanRead,
                        CanWrite = x.CanWrite,
                        CanDelete = x.CanDelete,
                        CanShare = x.CanShare,
                    }).ToList() ?? new List<FileSecurityDto>();

                if (data != null && data.Any())
                {
                    var one = data.First();
                    var users = _aspNetUsersRepository.GetQueryable().ToList();
                    var roles = _roleRepository.GetQueryable().ToList();
                    var department = _departmentRepository.GetQueryable().ToList();

                    foreach (var item in data)
                    {
                        if (item.SharedToType == FileManagerShareTypeConstant.USER)
                        {
                            item.SharedToName = users.FirstOrDefault(x => x.Id == item.SharedToID)?.Name ?? string.Empty;
                        }
                        else if (item.SharedToType == FileManagerShareTypeConstant.ROLE)
                        {
                            item.SharedToName = roles.FirstOrDefault(x => x.Id == item.SharedToID)?.Name ?? string.Empty;
                        }
                        else if (item.SharedToType == FileManagerShareTypeConstant.DEPARTMENT)
                        {
                            item.SharedToName = department.FirstOrDefault(x => x.Id == item.SharedToID)?.Name ?? string.Empty;
                        }
                        else item.SharedToName = string.Empty;
                    }
                }

                return data;
            }
            catch (Exception)
            {
                return new List<FileSecurityDto>();
            }
        }



        #endregion Cập nhật file or folder
        /// <summary>
        /// Tải file zip
        /// </summary>
        /// <param name="fileIDs">id những file/folder cần tải</param>
        /// <param name="rootPath">Thư mục gốc vật lý chứa file/folder</param>
        /// <param name="rootZipPath">Thư mục gốc vật lý chứa file/folder sau khí nén</param>
        /// <returns></returns>
        public async Task<string> Download(List<Guid> fileIDs, string rootPath, string rootZipPath)
        {
            var result = "";
            try
            {
                if (!fileIDs.Any()) return result;

                var fileDownloads = new List<FileManager>();
                foreach (var id in fileIDs)
                {
                    var item = await GetByIdAsync(id);
                    if (item != null)
                        fileDownloads.Add(item);
                }

                if (!fileDownloads.Any()) return result;

                // Tạo thư mục tạm chứa tất cả file/folder cần nén
                var tempFolder = Path.Combine(rootZipPath, $"temp_{Guid.NewGuid()}");
                Directory.CreateDirectory(tempFolder);

                var allItems = new List<FileManager>();
                var allFileManagers = GetQueryable().ToList();

                foreach (var item in fileDownloads)
                {
                    var removeHeaderParent =
                        item.Path.Substring(0, item.Path.LastIndexOf(item.Name))
                        .TrimEnd('/');

                    item.Path = "/" + item.Name;
                    var childItems = await GetChildToZip(item.Id, allFileManagers, removeHeaderParent);
                    allItems.AddRange(childItems);
                }

                allItems.AddRange(fileDownloads);

                // Copy tất cả vào thư mục tạm trước khi nén
                foreach (var item in allItems)
                {
                    if (string.IsNullOrEmpty(item.PhysicalPath)) continue;

                    var sourcePath = Path.Combine(rootPath, item.PhysicalPath);
                    var relativeZipPath = item.Path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
                    var targetPath = Path.Combine(tempFolder, relativeZipPath);

                    if (item.IsDirectory == true)
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    else
                    {
                        var parentDir = Path.GetDirectoryName(targetPath);
                        if (!Directory.Exists(parentDir)) Directory.CreateDirectory(parentDir);

                        if (System.IO.File.Exists(sourcePath))
                            System.IO.File.Copy(sourcePath, targetPath, overwrite: true);
                    }
                }

                // Nén toàn bộ thư mục tạm thành file zip
                var zipFileNameFinal = $"download_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                var zipFilePath = Path.Combine(rootZipPath, zipFileNameFinal);
                ZipFile.CreateFromDirectory(tempFolder, zipFilePath, CompressionLevel.Fastest, includeBaseDirectory: false);

                // Xóa thư mục tạm
                Directory.Delete(tempFolder, recursive: true);

                result = zipFilePath;
            }
            catch (Exception ex)
            {
                return "";
            }

            return result;
        }

        public async Task<List<FileManagerDto>> GetDataAll(FileManagerSearch search)
        {

            var fileSecurities = _fileSecurityRepository.GetQueryable();

            // thông tin chia sẻ cho người dùng
            var userFilePermisons = fileSecurities?
                .Where(x => x.SharedToType == FileManagerShareTypeConstant.USER
                && x.SharedToID == search.UserID);

            // thông tin chia sẻ cho nhóm quyền
            var roleFilePermisons = fileSecurities?
                .Where(x => x.SharedToType == FileManagerShareTypeConstant.ROLE)
                .Join(_userRoleRepository.GetQueryable().Where(x => x.UserId == search.UserID),
                s => s.SharedToID,
                ul => ul.RoleId,
                (s, ul) => new { s })
                .Select(x => x.s);

            // thông tin chia sẻ cho phòng ban
            var departmentFilePermisons = fileSecurities?
                .Where(x => x.SharedToType == FileManagerShareTypeConstant.DEPARTMENT);
            if (search.UserDepartmentID != null)
            {
                departmentFilePermisons = departmentFilePermisons
                    .Where(x => x.SharedToID == search.UserDepartmentID);
            }

            // sau khi lọc theo từng type để join thì gộp lại thành 1
            var allPermissions = userFilePermisons
               .Concat(roleFilePermisons)
               .Concat(departmentFilePermisons);

            var query = from q in GetQueryable()
                        join p in allPermissions on q.Id equals p.FileID into pGroup

                        select new FileManagerDto()
                        {
                            Name = q.Name,
                            ParentId = q.ParentId,
                            Path = q.Path,
                            Size = q.Size,
                            IsDirectory = q.IsDirectory,
                            FileExtension = q.FileExtension,
                            PhysicalPath = q.PhysicalPath,
                            MimeType = q.MimeType,
                            CreatedBy = q.CreatedBy,
                            UpdatedBy = q.UpdatedBy,
                            IsDelete = q.IsDelete,
                            DeleteId = q.DeleteId,
                            CreatedDate = q.CreatedDate,
                            UpdatedDate = q.UpdatedDate,
                            DeleteTime = q.DeleteTime,
                            Id = q.Id,
                            Permission = new FilePermissionDto()
                            {
                                Copy = !pGroup.Any() || pGroup.Any(x => x.CanWrite),
                                Move = !pGroup.Any() || pGroup.Any(x => x.CanWrite),
                                Create = !pGroup.Any() || pGroup.Any(x => x.CanWrite),
                                Upload = !pGroup.Any() || pGroup.Any(x => x.CanWrite),
                                Delete = !pGroup.Any() || pGroup.Any(x => x.CanDelete),
                                Download = !pGroup.Any() || pGroup.Any(x => x.CanRead),
                                Rename = !pGroup.Any() || pGroup.Any(x => x.CanWrite),
                                Share = !pGroup.Any() || pGroup.Any(x => x.CanShare)
                            }

                        };
            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.Name))
                {
                    query = query.Where(x => EF.Functions.Like(x.Name, $"%{search.Name}%"));
                }
                if (search.ParentId.HasValue)
                {
                    query = query.Where(x => x.ParentId == search.ParentId || x.Id == search.ParentId);
                } else
                {
                    query = query.Where(x => x.ParentId == null || x.ParentId == new Guid());
                }
                if (search.IsDirectory.HasValue)
                {
                    query = query.Where(x => x.IsDirectory == search.IsDirectory);
                }

            }
            query = query.OrderByDescending(x => x.CreatedDate);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<PagedList<FileManagerDto>> GetData(FileManagerSearch search)
        {
            var query = from q in GetQueryable()

                        select new FileManagerDto()
                        {
                            Name = q.Name,
                            ParentId = q.ParentId,
                            Path = q.Path,
                            Size = q.Size,
                            IsDirectory = q.IsDirectory,
                            FileExtension = q.FileExtension,
                            MimeType = q.MimeType,
                            CreatedBy = q.CreatedBy,
                            UpdatedBy = q.UpdatedBy,
                            IsDelete = q.IsDelete,
                            DeleteId = q.DeleteId,
                            CreatedDate = q.CreatedDate,
                            UpdatedDate = q.UpdatedDate,
                            DeleteTime = q.DeleteTime,
                            Id = q.Id,
                        };
            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.Name))
                {
                    query = query.Where(x => EF.Functions.Like(x.Name, $"%{search.Name}%"));
                }
                if (search.ParentId.HasValue)
                {
                    query = query.Where(x => x.ParentId == search.ParentId);
                }

                if (search.IsDirectory.HasValue)
                {
                    query = query.Where(x => x.IsDirectory == search.IsDirectory);
                }

            }
            query = query.OrderByDescending(x => x.CreatedDate);
            var result = await PagedList<FileManagerDto>.CreateAsync(query, search);
            return result;
        }

        public async Task<FileManagerDto?> GetDto(Guid id)
        {
            var item = await (from q in GetQueryable().Where(x => x.Id == id)

                              select new FileManagerDto()
                              {
                                  Name = q.Name,
                                  ParentId = q.ParentId,
                                  Path = q.Path,
                                  Size = q.Size,
                                  IsDirectory = q.IsDirectory,
                                  FileExtension = q.FileExtension,
                                  MimeType = q.MimeType,
                                  CreatedBy = q.CreatedBy,
                                  UpdatedBy = q.UpdatedBy,
                                  IsDelete = q.IsDelete,
                                  DeleteId = q.DeleteId,
                                  CreatedDate = q.CreatedDate,
                                  UpdatedDate = q.UpdatedDate,
                                  DeleteTime = q.DeleteTime,
                                  Id = q.Id,
                              }).FirstOrDefaultAsync();

            return item;
        }

        public Task<bool> SaveSecurity(List<BaseProject.Model.Entities.FileSecurity> fileSecurities, Guid fileID)
        {
            throw new NotImplementedException();
        }
    }
}
