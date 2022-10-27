import { useState, useEffect } from "react";
import { getData, postData } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";

const CreateRomooc = (props) => {
  const { getListRomooc, listStatus } = props;

  const [IsLoading, SetIsLoading] = useState(true);
  const {
    register,
    reset,
    control,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const Validate = {
    MaRomooc: {
      required: "Không được để trống",
      maxLength: {
        value: 10,
        message: "Không được vượt quá 10 ký tự",
      },
      minLength: {
        value: 6,
        message: "Không được ít hơn 6 ký tự",
      },
      pattern: {
        value: /^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$/,
        message: "Không được chứa ký tự đặc biệt",
      },
    },
    SoGuRomooc: {
      required: "Không được bỏ trống",
    },
    KetCauSan: {
      required: "Không được bỏ trống",
    },
    ThongSoKyThuat: {
      required: "Không được bỏ trống",
    },
    LoaiRomooc: {
      required: "Không được bỏ trống",
    },
    TrangThai: {
      required: "Không được bỏ trống",
    },
  };

  const [listRomoocType, setListRomoocType] = useState([]);

  useEffect(() => {
    SetIsLoading(true);
    (async () => {
      let getListRomoocType = await getData("Romooc/GetListSelectRomoocType");
      setListRomoocType(getListRomoocType);
      SetIsLoading(false);
    })();
  }, []);

  const onSubmit = async (data) => {
    SetIsLoading(true);
    const post = await postData("Romooc/CreateRomooc", {
      MaRomooc: data.MaRomooc,
      KetCauSan: data.KetCauSan,
      SoGuRomooc: data.SoGuRomooc,
      ThongSoKyThuat: data.ThongSoKyThuat,
      MaLoaiRomooc: data.LoaiRomooc,
    });

    if (post === 1) {
      getListRomooc(1);
      handleResetClick();
    }
    SetIsLoading(false);
  };

  const handleResetClick = () => {
    reset();
  };

  return (
    <>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">Form Thêm Mới Romooc</h3>
        </div>
        <div>{IsLoading === true && <div>Loading...</div>}</div>
        {IsLoading === false && (
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="card-body">
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="MaRomooc">Mã Romooc</label>
                    <input
                      autoComplete="false"
                      type="text"
                      className="form-control"
                      id="MaRomooc"
                      {...register("MaRomooc", Validate.MaRomooc)}
                    />
                    {errors.MaRomooc && (
                      <span className="text-danger">
                        {errors.MaRomooc.message}
                      </span>
                    )}
                  </div>
                </div>

                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="SoGuRomooc">Số Gù Romooc</label>
                    <input
                      type="text"
                      className="form-control"
                      id="SoGuRomooc"
                      {...register("SoGuRomooc", Validate.SoGuRomooc)}
                    />
                    {errors.SoGuRomooc && (
                      <span className="text-danger">
                        {errors.SoGuRomooc.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="row">
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="KetCauSan">Kếu Cấu Sàn</label>
                    <input
                      type="text"
                      className="form-control"
                      id="KetCauSan"
                      {...register("KetCauSan", Validate.KetCauSan)}
                    />
                    {errors.KetCauSan && (
                      <span className="text-danger">
                        {errors.KetCauSan.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col-sm">
                  <div className="form-group">
                    <label htmlFor="ThongSoKyThuat">Thông Số Kỹ Thuật</label>
                    <input
                      type="text"
                      className="form-control"
                      id="ThongSoKyThuat"
                      {...register("ThongSoKyThuat", Validate.ThongSoKyThuat)}
                    />
                    {errors.ThongSoKyThuat && (
                      <span className="text-danger">
                        {errors.ThongSoKyThuat.message}
                      </span>
                    )}
                  </div>
                </div>
                <div className="col col-sm">
                  <div className="form-group">
                    <label htmlFor="LoaiRomooc">Phân Loại Romooc</label>
                    <select
                      className="form-control"
                      {...register("LoaiRomooc", Validate.LoaiRomooc)}
                    >
                      <option value="">Chọn Loại Romooc</option>
                      {listRomoocType &&
                        listRomoocType.map((val) => {
                          return (
                            <option
                              value={val.maLoaiRomooc}
                              key={val.maLoaiRomooc}
                            >
                              {val.tenLoaiRomooc}
                            </option>
                          );
                        })}
                    </select>
                    {errors.LoaiRomooc && (
                      <span className="text-danger">
                        {errors.LoaiRomooc.message}
                      </span>
                    )}
                  </div>
                </div>
              </div>
              {/* <div className="form-group">
                <label htmlFor="GhiChu">Ghi Chú</label>
                <input
                  type="text"
                  className="form-control"
                  id="GhiChu"
                  placeholder="Nhập ghi chú"
                  {...register("GhiChu")}
                />
                {errors.GhiChu && (
                  <span className="text-danger">{errors.GhiChu.message}</span>
                )}
              </div> */}
              <div className="form-group">
                <label htmlFor="TrangThai">Trạng Thái</label>
                <select
                  className="form-control"
                  {...register("TrangThai", Validate.TrangThai)}
                >
                  <option value="">Chọn Trạng Thái</option>
                  {listStatus &&
                    listStatus.map((val) => {
                      return (
                        <option value={val.statusId} key={val.statusId}>
                          {val.statusContent}
                        </option>
                      );
                    })}
                </select>
                {errors.TrangThai && (
                  <span className="text-danger">
                    {errors.TrangThai.message}
                  </span>
                )}
              </div>
            </div>
            <div className="card-footer">
              <div>
                <button
                  type="button"
                  onClick={() => handleResetClick()}
                  className="btn btn-warning"
                >
                  Làm mới
                </button>
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ float: "right" }}
                >
                  Thêm mới
                </button>
              </div>
            </div>
          </form>
        )}
      </div>
    </>
  );
};

export default CreateRomooc;
